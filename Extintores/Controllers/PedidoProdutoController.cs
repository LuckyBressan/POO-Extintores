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

            return CreatedAtAction("GetPedidoProduto", new { id = pedidoProduto.PedidoCodigo }, pedidoProduto);
        }

        // DELETE: api/PedidoProduto/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeletePedidoProduto(string codigo)
        {
            var codigos = TrataCodigoRecebido(codigo);
            var pedidoProduto = await _context.PedidoProduto.FindAsync(codigo);
            if (pedidoProduto == null)
            {
                return NotFound();
            }

            _context.PedidoProduto.Remove(pedidoProduto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoProdutoExists(List<int> codigos)
        {
            return _context.PedidoProduto.Any(e => e.PedidoCodigo == codigos[0] && e.ProdutoCodigo == codigos[1]);
        }

        private List<int> TrataCodigoRecebido(string codigo)
        {
            return codigo.Split('-') // separa em ["1", "1"]
                        .Select(int.Parse) // converte cada item para int
                        .ToList();
        }
    }
}
