using Agenda.Data;
using Agenda.DTO;
using Agenda.Models;
using DTO.requests;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class PessoaService(AgendaContext db)
    {
        public async Task<List<PessoaResponseDTO>> ListAllPessoas()
        {
            return await db.Pessoas
                .Where(u => u.IsActive)
                .Select(u => new PessoaResponseDTO(u))
                .ToListAsync();
        }

        public async Task<PessoaResponseDTO> FindByCPF(string cpf)
        {
            var pessoa = await FindByCPFInternal(cpf);
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

        public async Task<PessoaResponseDTO> CreatePessoa(PessoaRequestDTO novaPessoa)
        {
            if (await ExistsWithCPF(novaPessoa.CPF))
                throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");
            if (string.IsNullOrEmpty(novaPessoa.Name))
                throw new ArgumentException("Nome não pode ser nulo ou vazio.");
            if (novaPessoa.BirthDate == DateOnly.MinValue)
                throw new ArgumentException("Data de nascimento não pode ser nula.");

            Pessoa pessoa = new Pessoa(novaPessoa.Name, novaPessoa.CPF, novaPessoa.BirthDate);

            db.Pessoas.Add(pessoa);
            await db.SaveChangesAsync();

            return new PessoaResponseDTO(pessoa);
        }

        public async Task<PessoaResponseDTO> UpdatePessoa(string cpf, PessoaRequestDTO novaPessoa)
        {
            Pessoa existente = await FindByCPFInternal(cpf);

            if (!string.IsNullOrEmpty(novaPessoa.Name))
                existente.Name = novaPessoa.Name;

            if (!string.IsNullOrEmpty(novaPessoa.CPF))
            {
                if (await ExistsWithCPF(novaPessoa.CPF, existente.Id))
                    throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");
                existente.CPF = novaPessoa.CPF;
            }

            if (novaPessoa.BirthDate != DateOnly.MinValue)
                existente.BirthDate = novaPessoa.BirthDate;

            await db.SaveChangesAsync();
            return new PessoaResponseDTO(existente);
        }

        public async Task<bool> DeletePessoa(string cpf)
        {
            Pessoa existente = await FindByCPFInternal(cpf);
            existente.IsActive = false;
            await db.SaveChangesAsync();
            return true;
        }
    }
}