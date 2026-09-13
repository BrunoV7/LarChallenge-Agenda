using Agenda.Models;

namespace Agenda.DTOs
{
    public class TelefoneDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = null!;
        public TipoTelefone Tipo { get; set; }

        public TelefoneDto(Telefone telefone)
        {
            this.Id = telefone.Id;
            this.Numero = telefone.Numero;
            this.Tipo = telefone.Tipo;
        }
    }
    public class TelefoneResponse : TelefoneDto
    {
        public Guid IdPessoa { get; set; }
        public string Nome { get; set; }
        public TelefoneResponse(Telefone telefone)
        : base(telefone)
        {
            IdPessoa = telefone.Pessoa.Id;
            Nome = telefone.Pessoa.Name;
        }
    }
}