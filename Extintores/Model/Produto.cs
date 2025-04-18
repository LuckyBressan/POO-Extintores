using System.ComponentModel.DataAnnotations;

namespace Extintores.Model
{
    public class Produto
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Nome do produto é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome do produto deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(300, MinimumLength = 10, ErrorMessage = "Descrição do produto deve ter entre 10 e 300 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Preco do produto é obrigatório")]
        public decimal Preco { get; set; }

        /** @todo criar uma tabela específica para controle de quantidade de produtos **/
        [Required(ErrorMessage = "Quantidade de produtos é obrigatória")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O produto deve estar associado a uma categoria")]
        public int CategoriaCodigo { get; set; }

        public Categoria? Categoria { get; set; }
    }
}
