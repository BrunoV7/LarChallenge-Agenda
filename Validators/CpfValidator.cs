namespace Agenda.Validators
{
    public class CpfValidator
    {
        private const int TotalDigitos = 11;

        public string Normalizar(string? cpf)
        {
            if (cpf == null)
                return "";

            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        public bool IsValid(string? cpf)
        {
            var cpfLimpo = Normalizar(cpf);

            if (cpfLimpo.Length != TotalDigitos)
                return false;

            if (cpfLimpo.All(c => c == cpfLimpo[0]))
                return false;

            var digitos = cpfLimpo.Select(c => c - '0').ToArray();

            var primeiroVerificador = CalcularDigito(digitos, 9);
            var segundoVerificador = CalcularDigito(digitos, 10);

            return primeiroVerificador == digitos[9]
                && segundoVerificador == digitos[10];
        }

        private static int CalcularDigito(int[] digitos, int quantidade)
        {
            var soma = 0;
            var peso = quantidade + 1;

            for (int i = 0; i < quantidade; i++)
            {
                soma += digitos[i] * peso;
                peso--;
            }

            var resto = soma % 11;
            if (resto < 2)
                return 0;
            return 11 - resto;
        }
    }
}