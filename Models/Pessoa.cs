namespace Agenda.Models
{
    public class Pessoa
    {
        public Guid Id { get; set;}
        public string Name { get; set;} = "";
        public string CPF { get; set;} = "";
        public  DateOnly BirthDate { get; set; }
        public bool isActive { get; set; } = true;

        public Pessoa(string _Name, string _CPF, DateOnly _BirthDate)
        {
            this.Name = _Name;
            this.CPF = _CPF;
            this.BirthDate = _BirthDate;
        }

        public Pessoa()
        {
        }
    }
}