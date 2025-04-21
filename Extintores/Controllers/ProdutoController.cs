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
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Produto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProduto()
        {
            return await _context.Produto.Include(p => p.Categoria).OrderBy(p => p.Codigo).ToListAsync();
        }

        // GET: api/Produto/5
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Produto>> GetProduto(int codigo)
        {
            var produto = await _context.Produto.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        // PUT: api/Produto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutProduto(int codigo, Produto produto)
        {
            if (codigo != produto.Codigo)
            {
                return BadRequest("Produto sendo atualizado não condiz com o informado");
            }
            if (!CategoriaExists(produto.CategoriaCodigo))
            {
                return BadRequest("Categoria de produto não existe!");
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(codigo))
                {
                    return NotFound("Produto não existe!");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Produto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (produto == null)
            {
                return BadRequest("Não foi passado um produto!");
            }
            if(!CategoriaExists(produto.CategoriaCodigo))
            {
                return BadRequest("Categoria de produto não existe!");
            }
            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { codigo = produto.Codigo }, produto);
        }

        // DELETE: api/Produto/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteProduto(int codigo)
        {
            var produto = await _context.Produto.FindAsync(codigo);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            _context.Produto.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int codigo)
        {
            return _context.Produto.Any(e => e.Codigo == codigo);
        }

        private bool CategoriaExists(int codigo)
        {
            return _context.Categoria.Any(c => c.Codigo == codigo);
        }
    }
}
