namespace Agenda.Models
{
    public class Pessoa
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly DataNascimento { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();

        public Pessoa(string nome, string cpf, DateOnly dataNascimento)
        {
            this.Id = Guid.CreateVersion7();
            this.Nome = nome;
            this.CPF = cpf;
            this.DataNascimento = dataNascimento;
        }

        public Pessoa(string nome, string cpf, DateOnly dataNascimento, ICollection<Telefone> telefones)
        {
            this.Id = Guid.CreateVersion7();
            this.Nome = nome;
            this.CPF = cpf;
            this.DataNascimento = dataNascimento;
            this.Telefones = telefones;
        }

        public Pessoa()
        {
        }
    }
}