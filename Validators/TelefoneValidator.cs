namespace Agenda.Validators
{
    public class TelefoneValidator
    {
        public bool IsValid(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            telefone = Normalizar(telefone);

            // Valida telefone brasileiro:
            // DDD 11-99 + fixo (8 dígitos, começa 2-5) ou celular (9 dígitos, começa com 9).
            var regex = new System.Text.RegularExpressions.Regex(@"^(?:[1-9]{2}(?:[2-5][0-9]{7}|9[6-9][0-9]{7}))$");
            return regex.IsMatch(telefone);
        }

        public string Normalizar(string? telefone)
        {
            if (telefone == null) return "";
            return new string(telefone.Where(char.IsDigit).ToArray());
        }
    }
}