using Agenda.Models;

namespace Agenda.DTOs
{
    public class PessoaResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly BirthDate { get; set; }
        public List<TelefoneDto> Telefones { get; set; } = new List<TelefoneDto>();

        public PessoaResponseDTO(Pessoa pessoa)
        {
            this.Id = pessoa.Id;
            this.Name = pessoa.Name;
            this.CPF = pessoa.CPF;
            this.BirthDate = pessoa.BirthDate;
            if (pessoa.Telefones != null)
            {
                this.Telefones = pessoa.Telefones
                    .Where(t => t.IsActive)
                    .Select(t => new TelefoneDto(t))
                    .ToList();
            }
        }
    }
}