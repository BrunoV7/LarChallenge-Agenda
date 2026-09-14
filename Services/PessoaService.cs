using Agenda.Data;
using Agenda.DTOs;
using Agenda.Models;
using Agenda.Validators;
using DTO.requests;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class PessoaService(AgendaContext db, CpfValidator cpfValidator, ILogger<PessoaService> logger)
    {
        public async Task<PagedResult<PessoaResponseDTO>> ListAllPessoas(int page, int size, string? nome = null, bool ativo = true, bool desc = false)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;
            if (size > 100) size = 100;

            var query = db.Pessoas.Where(p => p.IsActive == ativo);

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Name.Contains(nome));

            var totalItems = await query.CountAsync();

            var ordenada = desc
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name);

            var pessoas = await ordenada
                .Include(p => p.Telefones)
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

        public async Task<Pessoa> FindByCPFInternal(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var cpfNormalizado = cpfValidator.Normalizar(cpf);

            var pessoa = await db.Pessoas
                .Include(p => p.Telefones)
                .FirstOrDefaultAsync(u => u.CPF == cpfNormalizado && u.IsActive);
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

        private async Task<bool> ExistsWithCPF(string cpfNormalizado, Guid? ignorarId = null)
        {
            return await db.Pessoas
                .AnyAsync(u => u.CPF == cpfNormalizado && (ignorarId == null || u.Id != ignorarId));
        }

        public async Task<PessoaResponseDTO> CreatePessoa(PessoaRequestDTO novaPessoa)
        {
            if (string.IsNullOrWhiteSpace(novaPessoa.Name))
                throw new ArgumentException("Nome não pode ser nulo ou vazio.");
            if (novaPessoa.BirthDate == DateOnly.MinValue)
                throw new ArgumentException("Data de nascimento não pode ser nula.");

            if (!cpfValidator.IsValid(novaPessoa.CPF))
                throw new ArgumentException("CPF inválido.");

            var cpfNormalizado = cpfValidator.Normalizar(novaPessoa.CPF);

            if (await ExistsWithCPF(cpfNormalizado))
                throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");

            Pessoa pessoa = new Pessoa(novaPessoa.Name, cpfNormalizado, novaPessoa.BirthDate);

            db.Pessoas.Add(pessoa);
            await db.SaveChangesAsync();
            logger.LogInformation("Pessoa criada: {PessoaId}", pessoa.Id);
            return new PessoaResponseDTO(pessoa);
        }

        public async Task<PessoaResponseDTO> UpdatePessoa(string cpf, PessoaRequestDTO novaPessoa)
        {
            Pessoa existente = await FindByCPFInternal(cpf);

            if (!string.IsNullOrWhiteSpace(novaPessoa.Name))
                existente.Name = novaPessoa.Name;

            if (!string.IsNullOrWhiteSpace(novaPessoa.CPF))
            {
                if (!cpfValidator.IsValid(novaPessoa.CPF))
                    throw new ArgumentException("CPF inválido.");

                var cpfNormalizado = cpfValidator.Normalizar(novaPessoa.CPF);

                if (await ExistsWithCPF(cpfNormalizado, existente.Id))
                    throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");

                existente.CPF = cpfNormalizado;
            }

            if (novaPessoa.BirthDate != DateOnly.MinValue)
                existente.BirthDate = novaPessoa.BirthDate;

            await db.SaveChangesAsync();
            logger.LogInformation("Pessoa atualizada: {PessoaId}", existente.Id);
            return new PessoaResponseDTO(existente);
        }

        public async Task<PessoaResponseDTO> ReativarPessoa(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var cpfNormalizado = cpfValidator.Normalizar(cpf);

            var pessoa = await db.Pessoas
                .FirstOrDefaultAsync(u => u.CPF == cpfNormalizado);

            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");

            if (pessoa.IsActive)
                throw new ArgumentException("Esta pessoa já está ativa.");

            pessoa.IsActive = true;

            var telefones = await db.Telefones
                .Where(t => t.IdPessoa == pessoa.Id && !t.IsActive)
                .ToListAsync();

            foreach (var telefone in telefones)
                telefone.IsActive = true;

            await db.SaveChangesAsync();
            logger.LogInformation("Pessoa reativada: {PessoaId}", pessoa.Id);
            return new PessoaResponseDTO(pessoa);
        }
        public async Task<bool> DeletePessoa(string cpf)
        {
            Pessoa existente = await FindByCPFInternal(cpf);
            existente.IsActive = false;

            var telefones = await db.Telefones
                .Where(t => t.IdPessoa == existente.Id && t.IsActive)
                .ToListAsync();

            foreach (var telefone in telefones)
                telefone.IsActive = false;

            await db.SaveChangesAsync();
            logger.LogInformation("Pessoa desativada: {PessoaId}", existente.Id);
            return true;
        }
    }
}