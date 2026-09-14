using Agenda.DTOs;
using Agenda.Models;
using DTO.requests;

namespace Agenda.Services
{
    public interface ITelefoneService
    {
        Task<PagedResult<TelefoneResponse>> ListAllTelefones(int page, int size, TipoTelefone? tipo = null, bool ativo = true, bool desc = false);
        Task<TelefoneResponse> FindById(Guid id);
        Task<PagedResult<TelefoneResponse>> FindAllTelefonesByCpf(string cpf, int page, int size);
        Task<List<TelefoneResponse>> FindByNumero(string numero);
        Task<Telefone> FindByIdInternal(Guid id);
        Task<bool> ExistsById(Guid id);
        Task<TelefoneResponse> CreateTelefone(TelefoneCreationRequest novoTelefone);
        Task<TelefoneResponse> UpdateTelefone(Guid id, TelefoneUpdateRequest telefoneAtualizado);
        Task DeleteTelefone(Guid id);
    }
}