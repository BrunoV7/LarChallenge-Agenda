using Agenda.Validators;

namespace Agenda.Tests;

public class CpfValidationTests
{
    private readonly CpfValidator validator = new();

    [Theory]
    [InlineData("880.555.510-06")] 
    [InlineData("33579059068")]    
    public void IsValid_ComCpfValido_RetornaTrue(string cpf)
    {
        Assert.True(validator.IsValid(cpf));
    }

    [Fact]
    public void IsValid_ComCpfInvalido_RetornaFalse()
    {
        var cpfInvalido = "877.261.120-20";

        var resultado = validator.IsValid(cpfInvalido);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("00000000000")]
    [InlineData("99999999999")]
    public void IsValid_ComDigitosRepetidos_RetornaFalse(string cpf)
    {
        Assert.False(validator.IsValid(cpf));
    }

    [Fact]
    public void IsValid_ComTamanhoInvalido_RetornaFalse()
    {
        var cpfInvalido = "00.418.920/0001-24";
        Assert.False(validator.IsValid(cpfInvalido));
    }

    [Fact]
    public void IsValid_ComLetras_RetornaFalse()
    {
        var cpfInvalido = "A93.096.840-D0";
        Assert.False(validator.IsValid(cpfInvalido));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_ComEntradaVaziaOuNula_RetornaFalse(string? cpf)
    {
        Assert.False(validator.IsValid(cpf));
    }
}