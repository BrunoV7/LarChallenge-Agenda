using Agenda.Data;
using Agenda.DTO;
using Agenda.Models;
using DTO.requests;

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
            List<PessoaResponseDTO> pessoas = db.Pessoas.Where(u => u.isActive).Select(u => new PessoaResponseDTO(u)).ToList();
            return pessoas;
        }

        public async Task<PessoaResponseDTO> FindByCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var pessoa = db.Pessoas.FirstOrDefault(u => u.CPF == cpf && u.isActive);
            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");
            return new PessoaResponseDTO(pessoa);
        }

        private async Task<Pessoa> FindByCPFInternal(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            var pessoa = db.Pessoas.FirstOrDefault(u => u.CPF == cpf && u.isActive);
            if (pessoa == null)
                throw new KeyNotFoundException("Pessoa não encontrada.");
            return pessoa;
        }

        private async Task<Boolean> ExistsWithCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            return db.Pessoas.Any(u => u.CPF == cpf);
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

        public async Task<PessoaResponseDTO> UpdatePessoa(string CPF, PessoaRequestDTO nova_pessoa)
        {
            Pessoa existente = await FindByCPFInternal(CPF);
            if (!string.IsNullOrEmpty(nova_pessoa.Name))
            {
                existente.Name = nova_pessoa.Name;
            }
            if (!string.IsNullOrEmpty(nova_pessoa.CPF))
            {
                if(await ExistsWithCPF(nova_pessoa.CPF))
                    throw new ArgumentException("Já existe um usuário cadastrado com este CPF.");
                existente.CPF = nova_pessoa.CPF;
            }
            if (nova_pessoa.BirthDate != DateOnly.MinValue)
            {
                existente.BirthDate = nova_pessoa.BirthDate;
            }
            await db.SaveChangesAsync();
            return new PessoaResponseDTO(existente);
        }

        public async Task<Boolean> DeletePessoa(string CPF)
        {
            Pessoa existente = await FindByCPFInternal(CPF);
            existente.isActive = false;
            await db.SaveChangesAsync();
            return true;
        }

    }
}