using Agenda.Models;

namespace Agenda.DTOs
{
    // DTO que é usada para cadastrar pessoas
    public class PessoaRequestDTO
    {
        public string Name { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly BirthDate { get; set; }
        
    }
}