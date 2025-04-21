using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Extintores.Model;
using Extintores.data;
using Extintores.Validations;

namespace Extintores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
        {
            return await _context.Cliente.ToListAsync();
        }

        // GET: api/Cliente/5
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Cliente>> GetCliente(int codigo)
        {
            var cliente = await _context.Cliente.FindAsync(codigo);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Cliente/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutCliente(int codigo, Cliente cliente)
        {
            if (codigo != cliente.Codigo)
            {
                return BadRequest("Produto sendo atualizado não condiz com o informado");
            }

            if (
                VerificaCpf(cliente)
            )
            {
                return BadRequest("Não foi informado um CPF válido");
            }
            else if (
                VerificaCnpj(cliente)
            )
            {
                return BadRequest("Não foi informado um CNPJ válido");
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(codigo))
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

        // POST: api/Cliente
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if(cliente == null)
            {
                return BadRequest("Não foi passado um produto!");
            }
            if(
                VerificaCpf(cliente)
            )
            {
                return BadRequest("Não foi informado um CPF válido");
            } 
            else if(
                VerificaCnpj(cliente)
            )
            {
                return BadRequest("Não foi informado um CNPJ válido");
            }

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.Codigo }, cliente);
        }

        // DELETE: api/Cliente/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteCliente(int codigo)
        {
            var cliente = await _context.Cliente.FindAsync(codigo);
            if (cliente == null)
            {
                return NotFound("Não foi encontrado o cliente informado!");
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int codigo)
        {
            return _context.Cliente.Any(e => e.Codigo == codigo);
        }

        private bool VerificaCpf(Cliente cliente)
        {
            return cliente.IsTipoFisica() && (cliente.Cpf == null || !ValidadorCPF.Validar(cliente.Cpf));
        }

        private bool VerificaCnpj(Cliente cliente)
        {
            return cliente.IsTipoJuridica() && (cliente.Cnpj == null || !ValidadorCNPJ.Validar(cliente.Cnpj));
        }
    }
}
