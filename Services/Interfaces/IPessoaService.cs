using Agenda.DTOs;
using Agenda.Models;
using DTO.requests;

namespace Agenda.Services
{
    public interface IPessoaService
    {
        Task<PagedResult<PessoaResponseDTO>> ListAllPessoas(int page, int size, string? nome = null, bool ativo = true, bool desc = false);
        Task<PessoaResponseDTO> FindByCPF(string cpf);
        Task<Pessoa> FindByCPFInternal(string cpf);
        Task<Pessoa> FindByIdInternal(Guid id);
        Task<PessoaResponseDTO> CreatePessoa(PessoaRequestDTO novaPessoa);
        Task<PessoaResponseDTO> UpdatePessoa(string cpf, PessoaUpdateRequest novaPessoa);
        Task<PessoaResponseDTO> ReativarPessoa(string cpf);
        Task<bool> DeletePessoa(string cpf);
    }
}