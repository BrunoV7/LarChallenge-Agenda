using System.Text.RegularExpressions;

namespace Agenda.Validators
{
    public partial class TelefoneValidator : ITelefoneValidator
    {
        // Valida telefone brasileiro:
        // DDD 11-99 + fixo (8 dígitos, começa 2-5) ou celular (9 dígitos, começa com 9).
        [GeneratedRegex(@"^(?:[1-9]{2}(?:[2-5][0-9]{7}|9[6-9][0-9]{7}))$")]
        private static partial Regex TelefoneRegex();
        public bool IsValid(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            telefone = Normalizar(telefone);
            return TelefoneRegex().IsMatch(telefone);
        }

        public string Normalizar(string? telefone)
        {
            if (telefone == null) return "";
            return new string(telefone.Where(char.IsDigit).ToArray());
        }
    }
}