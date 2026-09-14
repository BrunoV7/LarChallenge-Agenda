namespace Agenda.Validators
{
    public interface ITelefoneValidator
    {
        string Normalizar(string? telefone);
        bool IsValid(string? telefone);
    }
}