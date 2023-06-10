using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DesafioSoftwareEngineer01.Models;
using Newtonsoft.Json;
using DesafioSoftwareEngineer01.Util;

namespace DesafioSoftwareEngineer01.Controllers
{
    public class ClienteController : Controller
    {
        private readonly DBClassContext _context;

        public ClienteController(DBClassContext context)
        {
            _context = context;
        }

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            return _context.Clientes != null ?
                View(await _context.Clientes.Include(c => c.EnderecoCliente).ToListAsync()) :
                Problem("Entity set 'DBClassContext.Clientes' is null.");
        }

        // GET: Cliente/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.Include(c => c.EnderecoCliente).FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,NumeroTelefone,EnderecoCliente")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                Utils util = new Utils();
                cliente.NumeroTelefone = util.formatarStrign(cliente.NumeroTelefone);
                cliente.EnderecoCliente.Cep = util.formatarStrign(cliente.EnderecoCliente.Cep);

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Cliente/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.Include(c => c.EnderecoCliente).FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,NumeroTelefone,EnderecoCliente")] Cliente cliente)
        {
            
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recupera o cliente existente do banco de dados com o endereço associado, para manter o index e atualizar os dados
                    var clienteBanco = await _context.Clientes.Include(c => c.EnderecoCliente).FirstOrDefaultAsync(c => c.Id == cliente.Id);

                    if (clienteBanco == null)
                    {
                        return NotFound();
                    }
                    Utils util = new Utils();

                    clienteBanco.Name = cliente.Name;
                    clienteBanco.Email = cliente.Email;
                    clienteBanco.NumeroTelefone = util.formatarStrign(cliente.NumeroTelefone);  
                    clienteBanco.EnderecoCliente.Cep = util.formatarStrign(cliente.EnderecoCliente.Cep);
                    clienteBanco.EnderecoCliente.Logradouro = cliente.EnderecoCliente.Logradouro;
                    clienteBanco.EnderecoCliente.Complemento = cliente.EnderecoCliente.Complemento;
                    clienteBanco.EnderecoCliente.Bairro = cliente.EnderecoCliente.Bairro;
                    clienteBanco.EnderecoCliente.Localidade = cliente.EnderecoCliente.Localidade;
                    clienteBanco.EnderecoCliente.Uf = cliente.EnderecoCliente.Uf;
                    clienteBanco.EnderecoCliente.Ibge = cliente.EnderecoCliente.Ibge;
                    clienteBanco.EnderecoCliente.Gia = cliente.EnderecoCliente.Gia;
                    clienteBanco.EnderecoCliente.Ddd = cliente.EnderecoCliente.Ddd;
                    clienteBanco.EnderecoCliente.Siafi = cliente.EnderecoCliente.Siafi;  

                    _context.Update(clienteBanco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Cliente/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.Include(c => c.EnderecoCliente).FirstOrDefaultAsync(m => m.Id == id);
                
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'DBClassContext.Clientes'  is null.");
            }
            var cliente = await _context.Clientes.Include(c => c.EnderecoCliente).FirstOrDefaultAsync(m => m.Id == id);
            var enderecoCliente = await _context.Enderecos.FindAsync(cliente.EnderecoCliente.Id);
            if (cliente != null && enderecoCliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.Enderecos.Remove(enderecoCliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
          return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // POST: https://viacep.com.br/ws/{cep}/json/
        [HttpPost]
        public async Task<IActionResult> GetEnderecoPartial(string cep)
        {
            cep = cep.Replace("-", "");
            if (!string.IsNullOrEmpty(cep))
            {
                string url = $"https://viacep.com.br/ws/{cep}/json/";

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var endereco = JsonConvert.DeserializeObject<Endereco>(jsonString);
                        return Json(endereco);
                    }
                }
            }
            return Json(null);
        }
    }
}