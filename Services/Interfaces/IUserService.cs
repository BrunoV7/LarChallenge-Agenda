using Agenda.DTOs;

namespace Agenda.Services
{
    public interface IUserService
    {
        Task<List<UserResponse>> ListarUsuarios();
        Task<bool> DeletarUsuario(Guid id);
    }
}