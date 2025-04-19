using System.ComponentModel.DataAnnotations;

namespace Extintores.Model
{
    public class PedidoProduto
    {

        [Required(ErrorMessage = "Código do pedido é obrigatório")]
        public int PedidoCodigo { get; set; }

        public Pedido? Pedido { get; set; }

        [Required(ErrorMessage = "Código do produto é obrigatório")]
        public int ProdutoCodigo { get; set; }

        public Produto? Produto { get; set; }

        [Required(ErrorMessage = "Quantidade de produtos é obrigatória")]
        public int Quantidade { get; set; }

    }
}
