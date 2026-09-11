using Agenda.Models;

namespace Agenda.DTO
{
    // DTO que é usada para respostas da API, esconde dados sensiveis
    public class PessoaResponseDTO
    {
        public Guid Id { get; set;}
        public string Name { get; set;} = "";
        public DateOnly BirthDate { get; set;}

        public PessoaResponseDTO(Pessoa pessoa)
        {
            this.Id = pessoa.Id;
            this.Name = pessoa.Name;
            this.BirthDate = pessoa.BirthDate;
        }
    }
}