using Agenda.Models;

namespace DTO.requests
{
    // DTO que é usada para cadastrar pessoas
    public class PessoaRequestDTO
    {
        public string Name { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly BirthDate { get; set; }

        public Pessoa toPessoa()
        {
            return new Pessoa(this.Name, this.CPF, this.BirthDate);
        }
    }
}