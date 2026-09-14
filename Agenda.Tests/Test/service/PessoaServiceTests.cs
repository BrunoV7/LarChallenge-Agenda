using Agenda.Data;
using Agenda.DTOs;
using Agenda.Exceptions;
using Agenda.Models;
using Agenda.Services;
using Agenda.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Agenda.Tests
{
    public class PessoaServiceTests
    {
        // cria um DbContext novo em memória 
        private static AgendaContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<AgendaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AgendaContext(options);
        }

        // monta o service com dependências
        private static PessoaService CriarService(AgendaContext db)
        {
            return new PessoaService(db, new CpfValidator(), NullLogger<PessoaService>.Instance);
        }

        [Fact]
        public async Task CreatePessoa_ComDadosValidos_SalvaPessoa()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);
            var request = new PessoaRequestDTO
            {
                Nome = "João Silva",
                CPF = "11144477735",
                DataNascimento = new DateOnly(1990, 1, 1)
            };

            var resultado = await service.CreatePessoa(request);

            Assert.NotNull(resultado);
            Assert.Equal("João Silva", resultado.Nome);
            Assert.Equal(1, await db.Pessoas.CountAsync());
        }

        [Fact]
        public async Task CreatePessoa_ComCpfDuplicado_LancaConflictException()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);
            var request = new PessoaRequestDTO
            {
                Nome = "João Silva",
                CPF = "11144477735",
                DataNascimento = new DateOnly(1990, 1, 1)
            };
            await service.CreatePessoa(request);

            var request2 = new PessoaRequestDTO
            {
                Nome = "Outra Pessoa",
                CPF = "11144477735",
                DataNascimento = new DateOnly(1995, 5, 5)
            };

            await Assert.ThrowsAsync<ConflictException>(() => service.CreatePessoa(request2));
        }

        [Fact]
        public async Task CreatePessoa_ComDataFutura_LancaArgumentException()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);
            var request = new PessoaRequestDTO
            {
                Nome = "João Silva",
                CPF = "11144477735",
                DataNascimento = new DateOnly(2050, 1, 1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreatePessoa(request));
        }

        [Fact]
        public async Task CreatePessoa_ComCpfInvalido_LancaArgumentException()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);
            var request = new PessoaRequestDTO
            {
                Nome = "João Silva",
                CPF = "12345678900",
                DataNascimento = new DateOnly(1990, 1, 1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreatePessoa(request));
        }

        [Fact]
        public async Task FindByCPF_ComCpfInexistente_LancaKeyNotFoundException()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.FindByCPF("11144477735"));
        }

        [Fact]
        public async Task DeletePessoa_DesativaAPessoaEOsTelefones()
        {
            var db = CriarContextoEmMemoria();
            var service = CriarService(db);

            var request = new PessoaRequestDTO
            {
                Nome = "João",
                CPF = "11144477735",
                DataNascimento = new DateOnly(1990, 1, 1)
            };
            var pessoa = await service.CreatePessoa(request);
            var pessoaEntity = await db.Pessoas.FirstAsync();
            db.Telefones.Add(new Telefone(TipoTelefone.Celular, "11987654321", pessoaEntity));
            await db.SaveChangesAsync();

            await service.DeletePessoa("11144477735");

            var pessoaDb = await db.Pessoas.FirstAsync();
            var telefoneDb = await db.Telefones.FirstAsync();
            Assert.False(pessoaDb.IsActive);
            Assert.False(telefoneDb.IsActive);
        }
    }
}