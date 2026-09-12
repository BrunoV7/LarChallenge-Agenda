using Agenda.Data;
using Agenda.DTO;
using Agenda.Models;
using DTO.requests;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class PessoaService
    {
        private readonly AgendaContext db;
        public PessoaService(AgendaContext _db)
        {
            db = _db;
        }
        public async Task<List<PessoaResponseDTO>> ListAllPessoas()
        {
            var pessoas = await db.Pessoas
                .Where(u => u.IsActive)
                .Select(u => new PessoaResponseDTO(u))
                .ToListAsync();
            return pessoas;
        }

        public async Task<PessoaResponseDTO> FindByCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var pessoa = await db.Pessoas.FirstOrDefaultAsync(u => u.CPF == cpf && u.IsActive);
            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");
            return new PessoaResponseDTO(pessoa);
        }

        private async Task<Pessoa> FindByCPFInternal(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var pessoa = await db.Pessoas.FirstOrDefaultAsync(u => u.CPF == cpf && u.IsActive);
            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");
            return pessoa;
        }

        private async Task<bool> ExistsWithCPF(string cpf, Guid? ignorarId = null)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            return await db.Pessoas
                .AnyAsync(u => u.CPF == cpf && (ignorarId == null || u.Id != ignorarId));
        }

        public async Task<PessoaResponseDTO> CreatePessoa(PessoaRequestDTO nova_pessoa)
        {
            if (await ExistsWithCPF(nova_pessoa.CPF))
                throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");
            if (string.IsNullOrEmpty(nova_pessoa.Name))
                throw new ArgumentException("Nome não pode ser nulo ou vazio.");
            if (nova_pessoa.BirthDate == DateOnly.MinValue)
                throw new ArgumentException("Data de nascimento não pode ser nula.");

            Pessoa pessoa = new Pessoa(nova_pessoa.Name, nova_pessoa.CPF, nova_pessoa.BirthDate);

            db.Pessoas.Add(pessoa);
            await db.SaveChangesAsync();

            return new PessoaResponseDTO(pessoa);
        }

        public async Task<PessoaResponseDTO> UpdatePessoa(string CPF, PessoaRequestDTO novaPessoa)
        {
            Pessoa existente = await FindByCPFInternal(CPF);
            if (!string.IsNullOrEmpty(novaPessoa.Name))
            {
                existente.Name = novaPessoa.Name;
            }
            if (!string.IsNullOrEmpty(novaPessoa.CPF))
            {
                if (await ExistsWithCPF(novaPessoa.CPF, existente.Id))   // ignora ela mesma
                    throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");
                existente.CPF = novaPessoa.CPF;
            }
            if (novaPessoa.BirthDate != DateOnly.MinValue)
            {
                existente.BirthDate = novaPessoa.BirthDate;
            }
            await db.SaveChangesAsync();
            return new PessoaResponseDTO(existente);
        }

        public async Task<bool> DeletePessoa(string CPF)
        {
            Pessoa existente = await FindByCPFInternal(CPF);
            existente.IsActive = false;
            await db.SaveChangesAsync();
            return true;
        }

    }
}