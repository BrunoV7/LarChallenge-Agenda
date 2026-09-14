using Agenda.Data;
using Agenda.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services
{
    public class UserService(AgendaContext context, ILogger<UserService> logger)
    {
        public async Task<List<UserResponse>> ListarUsuarios()
        {
            var users = await context.User
                .Where(u => u.IsActive)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
            return users.Select(u => new UserResponse(u)).ToList();
        }

        public async Task<bool> DeletarUsuario(Guid id)
        {
            var user = await context.User.FindAsync(id);
            if (user is null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            user.IsActive = false;
            await context.SaveChangesAsync();
            logger.LogInformation("Usuário desativado: {UserId}", id);
            return true;
        }
    }
}