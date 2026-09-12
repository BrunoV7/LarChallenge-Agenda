using Agenda.Models;

namespace Agenda.DTO
{
    public class PessoaResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly BirthDate { get; set; }

        public PessoaResponseDTO(Pessoa pessoa)
        {
            this.Id = pessoa.Id;
            this.Name = pessoa.Name;
            this.CPF = pessoa.CPF;
            this.BirthDate = pessoa.BirthDate;
        }
    }
}