using System.ComponentModel.DataAnnotations;

namespace Extintores.Model
{
    public class Cliente
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Nome do cliente deve ter entre 4 e 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateOnly DataNascimento { get; set; }

        [Required(ErrorMessage = "E-mail do cliente é obrigatório")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tipo de cliente é obrigatório")]
        public EnumTipoCliente Tipo { get; set; } = EnumTipoCliente.Fisica;

        public string Cpf { get; set; }

        public string Cnpj { get; set; }
    }
}
