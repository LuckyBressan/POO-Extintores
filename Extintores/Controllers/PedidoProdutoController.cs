using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Extintores.Model;
using Extintores.data;

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
            return await _context.PedidoProduto.ToListAsync();
        }

        // GET: api/PedidoProduto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoProduto>> GetPedidoProduto(int id)
        {
            var pedidoProduto = await _context.PedidoProduto.FindAsync(id);

            if (pedidoProduto == null)
            {
                return NotFound();
            }

            return pedidoProduto;
        }

        // PUT: api/PedidoProduto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoProduto(int id, PedidoProduto pedidoProduto)
        {
            if (id != pedidoProduto.PedidoCodigo)
            {
                return BadRequest();
            }

            _context.Entry(pedidoProduto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoProdutoExists(id))
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
                if (PedidoProdutoExists(pedidoProduto.PedidoCodigo))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoProduto(int id)
        {
            var pedidoProduto = await _context.PedidoProduto.FindAsync(id);
            if (pedidoProduto == null)
            {
                return NotFound();
            }

            _context.PedidoProduto.Remove(pedidoProduto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoProdutoExists(int id)
        {
            return _context.PedidoProduto.Any(e => e.PedidoCodigo == id);
        }
    }
}
