namespace Agenda.Models
{
    public enum TipoTelefone
    {
        Celular,
        Residencial,
        Comercial
    }

    public class Telefone
    {
        public Guid Id { get; set; }
        public TipoTelefone Tipo { get; set; }
        public string Numero { get; set;} = null!;
        public Guid IdPessoa { get; set; }
        public Pessoa Pessoa { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public Telefone(TipoTelefone tipo, string numero, Guid idPessoa)
        {
            this.Id = Guid.CreateVersion7();
            this.Tipo = tipo;
            this.Numero = numero;
            this.IdPessoa = idPessoa;
        }

        public Telefone(TipoTelefone tipo, string numero, Pessoa pessoa)
        {
            this.Id = Guid.CreateVersion7();
            this.Tipo = tipo;
            this.Numero = numero;
            this.Pessoa = pessoa;
            this.IdPessoa = pessoa.Id;
        }

        public Telefone()
        {
        }
    }
}