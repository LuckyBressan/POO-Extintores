using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Extintores.Model
{
    public class Pedido
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Descrição do pedido é obrigatória")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Desrição do pedido deve ter entre 5 e 300 caracteres")]
        public string Descricao { get; set; }

        public EnumEntregaPedido Entrega { get; set; } = EnumEntregaPedido.Entrega;

        [StringLength(300, MinimumLength = 5, ErrorMessage = "Local da entrega deve ter entre 5 e 100 caracteres")]
        public string LocalEntrega { get; set; }

        [Required(ErrorMessage = "Codigo do cliente é obrigatório")]
        public int ClienteCodigo{ get; set; }

        public Cliente? Cliente { get; set; }

        public bool IsPedidoEntrega()
        {
            return Entrega == EnumEntregaPedido.Entrega;
        }

        public bool IsPedidoRetirada()
        {
            return Entrega == EnumEntregaPedido.Fisica;
        }
    }
}
