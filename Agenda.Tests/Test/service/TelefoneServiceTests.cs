using Agenda.Data;
using Agenda.Exceptions;
using Agenda.Models;
using Agenda.Services;
using Agenda.Validators;
using DTO.requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Agenda.Tests
{
    public class TelefoneServiceTests
    {
        private static AgendaContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<AgendaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AgendaContext(options);
        }

        // cria o service com um IPessoaService mockado que devolve a pessoa passada
        private static TelefoneService CriarService(AgendaContext db, Pessoa? pessoaExistente = null)
        {
            var pessoaServiceMock = new Mock<IPessoaService>();

            // quando o service pedir a pessoa por Id ou CPF, devolve a que passamos
            if (pessoaExistente != null)
            {
                pessoaServiceMock
                    .Setup(p => p.FindByIdInternal(It.IsAny<Guid>()))
                    .ReturnsAsync(pessoaExistente);
                pessoaServiceMock
                    .Setup(p => p.FindByCPFInternal(It.IsAny<string>()))
                    .ReturnsAsync(pessoaExistente);
            }
            else
            {
                // sem pessoa: simula "não encontrada" lançando a exceção
                pessoaServiceMock
                    .Setup(p => p.FindByIdInternal(It.IsAny<Guid>()))
                    .ThrowsAsync(new KeyNotFoundException("Pessoa não encontrada."));
            }

            return new TelefoneService(
                db,
                pessoaServiceMock.Object,
                new TelefoneValidator(),
                new CpfValidator(),
                NullLogger<TelefoneService>.Instance);
        }

        [Fact]
        public async Task CreateTelefone_ComDadosValidos_SalvaTelefone()
        {
            var db = CriarContextoEmMemoria();
            var pessoa = new Pessoa("João", "11144477735", new DateOnly(1990, 1, 1));
            var service = CriarService(db, pessoa);

            var request = new TelefoneCreationRequest
            {
                Tipo = TipoTelefone.Celular,
                Numero = "45984135947",
                IdPessoa = pessoa.Id
            };

            var resultado = await service.CreateTelefone(request);

            Assert.NotNull(resultado);
            Assert.Equal(1, await db.Telefones.CountAsync());
        }

        [Fact]
        public async Task CreateTelefone_ComNumeroDuplicado_LancaConflictException()
        {
            var db = CriarContextoEmMemoria();
            var pessoa = new Pessoa("João", "11144477735", new DateOnly(1990, 1, 1));
            var service = CriarService(db, pessoa);

            db.Telefones.Add(new Telefone(TipoTelefone.Celular, "45984135947", pessoa));
            await db.SaveChangesAsync();

            var request = new TelefoneCreationRequest
            {
                Tipo = TipoTelefone.Residencial,
                Numero = "45984135947", 
                IdPessoa = pessoa.Id
            };

            await Assert.ThrowsAsync<ConflictException>(() => service.CreateTelefone(request));
        }

        [Fact]
        public async Task CreateTelefone_ComNumeroInvalido_LancaArgumentException()
        {
            var db = CriarContextoEmMemoria();
            var pessoa = new Pessoa("João", "11144477735", new DateOnly(1990, 1, 1));
            var service = CriarService(db, pessoa);

            var request = new TelefoneCreationRequest
            {
                Tipo = TipoTelefone.Celular,
                Numero = "123",       
                IdPessoa = pessoa.Id
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTelefone(request));
        }

        [Fact]
        public async Task CreateTelefone_SemTipo_LancaArgumentException()
        {
            var db = CriarContextoEmMemoria();
            var pessoa = new Pessoa("João", "11144477735", new DateOnly(1990, 1, 1));
            var service = CriarService(db, pessoa);

            var request = new TelefoneCreationRequest
            {
                Tipo = null,             
                Numero = "45984135947",
                IdPessoa = pessoa.Id
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTelefone(request));
        }

        [Fact]
        public async Task CreateTelefone_ComPessoaInexistente_LancaKeyNotFoundException()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);  

            var request = new TelefoneCreationRequest
            {
                Tipo = TipoTelefone.Celular,
                Numero = "45984135947",
                IdPessoa = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateTelefone(request));
        }
    }
}