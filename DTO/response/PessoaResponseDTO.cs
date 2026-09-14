using Agenda.Models;

namespace Agenda.DTOs
{
    public class PessoaResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly DataNascimento { get; set; }
        public List<TelefoneDto> Telefones { get; set; } = new List<TelefoneDto>();

        public PessoaResponseDTO(Pessoa pessoa)
        {
            this.Id = pessoa.Id;
            this.Nome = pessoa.Nome;
            this.CPF = pessoa.CPF;
            this.DataNascimento = pessoa.DataNascimento;
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