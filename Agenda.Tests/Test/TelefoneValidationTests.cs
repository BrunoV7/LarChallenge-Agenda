using Agenda.Validators;

namespace Agenda.Tests
{
    public class TelefoneValidationTests
    {
        private readonly TelefoneValidator validator = new();

        [Theory]
        [InlineData("(45) 98413-5947")]   // celular com DDD, 9 dígitos, começa com 9
        [InlineData("(44) 98765-4321")]   // celular
        [InlineData("(45) 3578-3385")]    // fixo, começa com 3
        [InlineData("(11) 2345-6789")]    // fixo, começa com 2
        public void IsValid_ComTelefoneValido_RetornaTrue(string telefone)
        {
            Assert.True(validator.IsValid(telefone));
        }

        [Theory]
        [InlineData("(45) 1234-5678")]    // fixo começando com 1 (inválido)
        [InlineData("(44) 9876-5432")]    // 8 dígitos começando com 9 (celular incompleto)
        [InlineData("(45) 6578-3385")]    // fixo começando com 6 (reservado a móvel)
        [InlineData("(10) 3578-3385")]    // DDD inválido
        [InlineData("45 98413-594")]      // dígitos de menos
        public void IsValid_ComTelefoneInvalido_RetornaFalse(string telefone)
        {
            Assert.False(validator.IsValid(telefone));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsValid_ComEntradaVaziaOuNula_RetornaFalse(string? telefone)
        {
            Assert.False(validator.IsValid(telefone));
        }
    }
}