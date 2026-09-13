using Agenda.Data;
using Agenda.DTO;
using Agenda.Models;
using DTO.requests;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class TelefoneService(
        AgendaContext db,
        PessoaService pessoaService)
    {
        public async Task<PagedResult<TelefoneResponse>> ListAllTelefones(int page, int size)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;
            if (size > 100) size = 100;

            var query = db.Telefones.Where(t => t.IsActive);
            var totalItems = await query.CountAsync();

            var telefones = await query
                .Include(t => t.Pessoa)
                .OrderBy(t => t.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
                
            return new PagedResult<TelefoneResponse>
            {
                Items = telefones.Select(p => new TelefoneResponse(p)).ToList(),
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)size)
            };
        }

        public async Task<TelefoneResponse> FindById(Guid id)
        {
            var telefone = await FindByIdInternal(id);
            return new TelefoneResponse(telefone);
        }

        public async Task<Telefone> FindByIdInternal(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("O Campo de Id não pode estar vazio ou inválido");
            var telefone = await db.Telefones
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive && t.Pessoa.IsActive);
            if (telefone == null)
                throw new KeyNotFoundException("Telefone não encontrado");
            return telefone;
        }

        public async Task<bool> ExistsById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("O Campo de Id não pode estar vazio ou inválido");
            return await db.Telefones.AnyAsync(t => t.Id == id && t.IsActive && t.Pessoa.IsActive);
        }

        private async Task<bool> ExistsByNumero(string numero, Guid idPessoa, Guid? ignorarId = null)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O Campo de Numero não pode ser nulo ou vazio");

            return await db.Telefones
                .AnyAsync(t => t.Numero == numero
                            && t.IdPessoa == idPessoa
                            && (ignorarId == null || t.Id != ignorarId));
        }

        public async Task<TelefoneResponse> CreateTelefone(TelefoneCreationRequest novoTelefone)
        {
            var pessoa = await pessoaService.FindByIdInternal(novoTelefone.IdPessoa);

            if (string.IsNullOrWhiteSpace(novoTelefone.Numero))
                throw new ArgumentException("O Campo de Numero não pode ser nulo ou vazio");

            if (await ExistsByNumero(novoTelefone.Numero, pessoa.Id))
                throw new ArgumentException("Esta pessoa já possui um telefone com este número.");

            Telefone telefone = new Telefone(novoTelefone.Tipo, novoTelefone.Numero, pessoa);
            db.Telefones.Add(telefone);
            await db.SaveChangesAsync();

            return new TelefoneResponse(telefone);
        }

        public async Task<TelefoneResponse> UpdateTelefone(Guid id, TelefoneUpdateRequest telefoneAtualizado)
        {
            var telefone = await FindByIdInternal(id);

            if (!string.IsNullOrWhiteSpace(telefoneAtualizado.Numero) && telefoneAtualizado.Numero != telefone.Numero)
            {
                if (await ExistsByNumero(telefoneAtualizado.Numero, telefone.IdPessoa, id))
                    throw new ArgumentException("Esta pessoa já possui um telefone com este número.");
                telefone.Numero = telefoneAtualizado.Numero;
            }

            if (telefoneAtualizado.Tipo.HasValue)
                telefone.Tipo = telefoneAtualizado.Tipo.Value;

            await db.SaveChangesAsync();
            return new TelefoneResponse(telefone);
        }

        public async Task DeleteTelefone(Guid id)
        {
            var telefone = await FindByIdInternal(id);
            telefone.IsActive = false;
            await db.SaveChangesAsync();
        }

    }
}