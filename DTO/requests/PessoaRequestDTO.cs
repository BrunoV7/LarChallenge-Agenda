using System.ComponentModel.DataAnnotations;
using Agenda.Models;

namespace Agenda.DTOs
{
    // DTO que é usada para cadastrar pessoas
    public class PessoaRequestDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly DataNascimento { get; set; }

    }
}