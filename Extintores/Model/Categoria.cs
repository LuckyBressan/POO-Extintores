using System.ComponentModel.DataAnnotations;

namespace Extintores.Model
{
    public class Categoria
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome da categoria é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome da categoria deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(300, MinimumLength = 10, ErrorMessage = "Descrição da categoria deve ter entre 10 e 300 caracteres")]
        public string Descricao { get; set; }
    }
}
