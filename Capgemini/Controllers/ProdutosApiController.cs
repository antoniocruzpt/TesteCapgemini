using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Capgemini.Data;
using Capgemini.Models;
using System.IO;
using ExcelDataReader;

namespace Capgemini.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProdutosApi
        [HttpGet]
        public IEnumerable<Produto> GetProdutos()
        {
            var r = _context.Produtos.ToList();
            return _context.Produtos;
        }

        // GET: api/ProdutosApi/5
        [HttpGet("{id}")]
        public IActionResult GetProduto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = _context.Produtos.Find(id);

            if (produto == null)
            {
                return NotFound();
            }

            
            return Ok(produto);
        }



        [HttpPost]
        public IActionResult PostProduto([FromBody]  List<Produto> produto)
        {
            var today = DateTime.Now;
            var produtos = new List<Produto>();
            var prod = new List<ProdutoView>();

            foreach (var item in produto)
            {
                var i = 0;
                var ok = true;
                var erro = new List<string>();

                var data = item.DataEntrega;
                var descricao = item.NomedoProduto;
                var quant = item.Quantidade;
                var valor = item.ValorUnitario;

                if (data <= today)
                {
                    ok = false;
                    erro.Add("A data é anterior à data atual!");
                }
                if (data == null)
                {
                    ok = false;
                    erro.Add("A data não pode ser nula!");
                }
                if (descricao.Length > 50)
                {
                    ok = false;
                    erro.Add("A descrição contém mais de 50 caracteres (" + descricao.Length + ")!");
                }
                if (string.IsNullOrEmpty(descricao))
                {
                    ok = false;
                    erro.Add("A descrição não pode ser nula!");
                }
                if (quant <= 0)
                {
                    ok = false;
                    erro.Add("A quantidade não pode ser inferior a 1 unidade!");
                }
                if (quant == null)
                {
                    ok = false;
                    erro.Add("A quantidade não pode ser nula!");
                }
                if (valor <= 0)
                {
                    ok = false;
                    erro.Add("o valor não pode ser zero ou inferior a zero!");
                }
                if (valor == null)
                {
                    ok = false;
                    erro.Add("o valor não pode ser nulo!");
                }

                prod.Add(new ProdutoView()
                {
                    Error = erro.ToList(),
                    NomedoProduto = descricao,
                    Quantidade = quant,
                    ValorUnitario = valor,
                    DataEntrega = data,
                    Status = ok
                });
            }

            var isok = prod.Any(q => q.Status == false);

            if (isok)
            {
                return BadRequest(prod);
            }

            foreach(var item in prod)
            {
                var p = new Produto
                {
                    DataEntrega = item.DataEntrega,
                    NomedoProduto = item.NomedoProduto,
                    Quantidade = item.Quantidade,
                    ValorUnitario = Math.Round((double)item.ValorUnitario, 2) 
                };
                produtos.Add(p);
            }
            _context.AddRange(produtos);
            _context.SaveChanges();
            return Ok(prod);
        }

        private List<ProdutoView> GetProdutosList(string name)
        {
            var today = DateTime.Now;
            var produtos = new List<Produto>();
            var prod = new List<ProdutoView>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + name;
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();
                    while (reader.Read())
                    {
                        var i = 0;
                        var ok = true;
                        var erro = new string[4];

                        var data = DateTime.Parse(reader.GetValue(0).ToString());
                        var descricao = reader.GetValue(1).ToString();
                        var quant = Convert.ToInt32(reader.GetValue(2).ToString());
                        var valor = double.Parse(reader.GetValue(3).ToString());

                        if (data <= today)
                        {
                            ok = false;
                            erro[i] = "A data é anterior à data atual!";
                            i++;
                        }
                        if (descricao.Length > 50)
                        {
                            ok = false;
                            erro[i] = "A descrição contém mais de 50 caracteres (" + descricao.Length + ")!";
                            i++;
                        }
                        if (quant <= 0)
                        {
                            ok = false;
                            erro[i] = "A quantidade não pode ser inferior a 1 unidade!";
                            i++;
                        }
                        if (valor <= 0)
                        {
                            ok = false;
                            erro[i] = "o valor não pode ser zero ou inferior a zero!";
                        }

                        prod.Add(new ProdutoView()
                        {
                            Error = erro.ToList(),
                            NomedoProduto = descricao,
                            Quantidade = quant,
                            ValorUnitario = valor,
                            DataEntrega = data,
                            Status = ok
                        });


                        if (ok)
                        {
                            produtos.Add(new Produto()
                            {
                                DataEntrega = data,
                                NomedoProduto = descricao,
                                Quantidade = quant,
                                ValorUnitario = valor
                            });
                        }
                    }
                }
            }
            return prod;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(produto);
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}