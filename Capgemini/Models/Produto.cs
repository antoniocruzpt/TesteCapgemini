using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capgemini.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }
        public DateTime? DataEntrega { get; set; }
        public string NomedoProduto { get; set; }
        public int? Quantidade { get; set; }
        public double? ValorUnitario { get; set; }
    }
    public class ProdutoView
    {
        [Key]
        public int Id { get; set; }
        public DateTime? DataEntrega { get; set; }
        public string NomedoProduto { get; set; }
        public int? Quantidade { get; set; }
        public double? ValorUnitario { get; set; }
        public bool Status { get; set; }
        public List<string> Error { get; set; }
    }

}
