using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Extintores.Model;
using Extintores.data;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Extintores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {
            return await _context.Pedido.Include(p => p.Cliente).OrderBy(p => p.Codigo).ToListAsync();
        }

        // GET: api/Pedido/5
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Pedido>> GetPedido(int codigo)
        {
            var pedido = await _context.Pedido.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        // PUT: api/Pedido/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutPedido(int codigo, Pedido pedido)
        {
            if (codigo != pedido.Codigo)
            {
                return BadRequest("Produto sendo atualizado não condiz com o informado");
            }

            if(!ClienteExists(pedido.ClienteCodigo))
            {
                return BadRequest("Cliente informado para o pedido não existe");
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(codigo))
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

        // POST: api/Pedido
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            if(pedido == null)
            {
                return BadRequest("Não foi passado um pedido!");
            }

            if (!ClienteExists(pedido.ClienteCodigo))
            {
                return BadRequest("Cliente informado para o pedido não existe");
            }

            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.Codigo }, pedido);
        }

        // DELETE: api/Pedido/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedido.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedido.Any(e => e.Codigo == id);
        }

        private bool ClienteExists(int codigo)
        {
            return _context.Cliente.Any(c => c.Codigo == codigo);
        }
    }
}
