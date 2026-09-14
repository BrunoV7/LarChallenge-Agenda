using Agenda.Models;

namespace Agenda.DTOs
{
    // DTO que é usada para cadastrar pessoas
    public class PessoaRequestDTO
    {
        public string Nome { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly DataNascimento { get; set; }
        
    }
}