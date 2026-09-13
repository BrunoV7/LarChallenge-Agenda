using Agenda.Data;
using Agenda.DTO;
using Agenda.Models;
using DTO.requests;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class PessoaService(AgendaContext db)
    {
        public async Task<PagedResult<PessoaResponseDTO>> ListAllPessoas(int page, int size)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;
            if (size > 100) size = 100;

            var query = db.Pessoas.Where(p => p.IsActive);
            var totalItems = await query.CountAsync();       

            var pessoas = await query
                .Include(p => p.Telefones)                     
                .OrderBy(p => p.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedResult<PessoaResponseDTO>
            {
                Items = pessoas.Select(p => new PessoaResponseDTO(p)).ToList(),
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)size)
            };
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

            var pessoa = await db.Pessoas
                .Include(p => p.Telefones)
                .FirstOrDefaultAsync(u => u.CPF == cpf && u.IsActive);
            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");

            return pessoa;
        }

        public async Task<Pessoa> FindByIdInternal(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("O Campo de Id não pode estar vazio ou inválido");

            var pessoa = await db.Pessoas.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
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