namespace Agenda.Models
{
    public class Pessoa
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string CPF { get; set; } = "";
        public DateOnly BirthDate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();

        public Pessoa(string name, string cpf, DateOnly birthDate)
        {
            this.Id = Guid.CreateVersion7();
            this.Name = name;
            this.CPF = cpf;
            this.BirthDate = birthDate;
        }

        public Pessoa(string name, string cpf, DateOnly birthDate, ICollection<Telefone> telefones)
        {
            this.Id = Guid.CreateVersion7();
            this.Name = name;
            this.CPF = cpf;
            this.BirthDate = birthDate;
            this.Telefones = telefones;
        }

        public Pessoa()
        {
        }
    }
}