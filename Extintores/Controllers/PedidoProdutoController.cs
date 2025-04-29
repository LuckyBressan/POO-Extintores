using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Extintores.Model;
using Extintores.data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Extintores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidoProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PedidoProduto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoProduto>>> GetPedidoProduto()
        {
            return await _context.PedidoProduto.Include(pp => pp.Pedido).Include(pp => pp.Produto).OrderBy(pp => pp.PedidoCodigo).ToListAsync();
        }

        // GET: api/PedidoProduto/5
        [HttpGet("{codigo}")]
        public async Task<ActionResult<PedidoProduto>> GetPedidoProduto(string codigo)
        {
            var codigos = TrataCodigoRecebido(codigo);

            var pedidoProduto = await _context.PedidoProduto.Include(pp => pp.Pedido).Include(pp => pp.Produto).FirstOrDefaultAsync(pp => pp.PedidoCodigo == codigos[0] && pp.ProdutoCodigo == codigos[1]);

            if (pedidoProduto == null)
            {
                return NotFound();
            }

            return pedidoProduto;
        }

        // PUT: api/PedidoProduto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutPedidoProduto(string codigo, PedidoProduto pedidoProduto)
        {
            if (codigo == null)
            {
                return BadRequest();
            }

            if (
                !ProdutoExists(pedidoProduto.ProdutoCodigo) ||
                !PedidoExists(pedidoProduto.PedidoCodigo)
            )
            {
                return BadRequest("Produto ou Pedido informado não existe!");
            }

            var pedidoProdutoUpdate = await FindPedidoProduto(TrataCodigoRecebido(codigo));

            if(pedidoProdutoUpdate == null)
            {
                return BadRequest("Pedido Produto informado não existe");
            }

            //Restauramos o estoque para antes da inclusão do pedido
            await ReverteEstoqueProduto(pedidoProdutoUpdate.ProdutoCodigo, pedidoProdutoUpdate.Quantidade);

            if (!(await RetiraEstoqueProduto(pedidoProduto.ProdutoCodigo, pedidoProduto.Quantidade)))
            {
                return BadRequest("Não foi possível atualizar o estoque!");
            }

            var codigos = TrataCodigoRecebido(codigo);

            _context.Entry(pedidoProduto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoProdutoExists(codigos))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PedidoProduto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PedidoProduto>> PostPedidoProduto(PedidoProduto pedidoProduto)
        {
            if(
                !ProdutoExists(pedidoProduto.ProdutoCodigo) ||
                !PedidoExists(pedidoProduto.PedidoCodigo)
            )
            {
                return BadRequest("Produto ou Pedido informado não existe!");
            }

            if(!(await RetiraEstoqueProduto(pedidoProduto.ProdutoCodigo, pedidoProduto.Quantidade)))
            {
                return BadRequest("Não foi possível atualizar o estoque do produto!");
            }

            _context.PedidoProduto.Add(pedidoProduto);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                List<int> codigos = new List<int> { pedidoProduto.PedidoCodigo, pedidoProduto.ProdutoCodigo };
                if (PedidoProdutoExists(codigos))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPedidoProduto", new { id = pedidoProduto.PedidoCodigo }, pedidoProduto.ProdutoCodigo);
        }

        // DELETE: api/PedidoProduto/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeletePedidoProduto(string codigo)
        {
            var pedidoProduto = await FindPedidoProduto(TrataCodigoRecebido(codigo));
            if (pedidoProduto == null)
            {
                return NotFound();
            }

            _context.PedidoProduto.Remove(pedidoProduto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<PedidoProduto> FindPedidoProduto(List<int> codigos)
        {
            return await _context.PedidoProduto.Where(pp => pp.ProdutoCodigo == codigos[0] && pp.PedidoCodigo == codigos[1]).FirstAsync();
        }

        private bool PedidoProdutoExists(List<int> codigos)
        {
            return _context.PedidoProduto.Any(e => e.PedidoCodigo == codigos[0] && e.ProdutoCodigo == codigos[1]);
        }

        private bool ProdutoExists(int codigo)
        {
            return _context.Produto.Any(e => e.Codigo == codigo);
        }

        private bool PedidoExists(int codigo)
        {
            return _context.Pedido.Any(e => e.Codigo == codigo);
        }

        private bool ValidaQuantidadeInvalidaProduto(int codigo, int quantidadeSolicitada)
        {
            var produto = _context.Produto.Where(p => p.Codigo == codigo).First();

            return quantidadeSolicitada > produto.Quantidade;
        }

        private async Task<bool> ReverteEstoqueProduto(int codigo, int quantidade)
        {
            var produto = await _context.Produto.Where(p => p.Codigo == codigo).FirstAsync();

            produto.Quantidade += quantidade;

            return await AtualizaEstoqueProduto(produto);
        }

        private async Task<bool> RetiraEstoqueProduto(int codigo, int quantidade)
        {
            var produto = await _context.Produto.Where(p => p.Codigo == codigo).FirstAsync();

            if(ValidaQuantidadeInvalidaProduto(produto.Codigo, quantidade))
            {
                return false;
            }

            produto.Quantidade -= quantidade;

            return await AtualizaEstoqueProduto(produto);
        }

        private async Task<bool> AtualizaEstoqueProduto(Produto produto)
        {

            try
            {
                _context.Produto.Update(produto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        private List<int> TrataCodigoRecebido(string codigo)
        {
            return codigo.Split('-') // separa em ["1", "1"]
                        .Select(int.Parse) // converte cada item para int
                        .ToList();
        }
    }
}
