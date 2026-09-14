namespace Agenda.Validators
{
    public interface ICpfValidator
    {
        string Normalizar(string? cpf);
        bool IsValid(string? cpf);
    }
}