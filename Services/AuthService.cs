using Agenda.Data;
using Agenda.DTOs;
using Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services;

public class AuthService(AgendaContext context, TokenService tokenService, ILogger<AuthService> logger)
{
    public async Task<TokenResponse?> Register(UserRegisterRequest request)
    {
        if (await context.User.AnyAsync(u => u.Email == request.Email))
            return null; 

        var user = new User
        {
            Nome = request.Nome,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        };

        context.User.Add(user);
        await context.SaveChangesAsync();
        logger.LogInformation("Novo usuário registrado: {UserId}", user.Id);
        return tokenService.GerarToken(user);
    }

    public async Task<TokenResponse?> Login(UserLoginRequest request)
    {
        var user = await context.User
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Tentativa de login falha: {Email}", request.Email);
            return null; 
        }
        logger.LogInformation("Login bem-sucedido: {Email}", request.Email);
        return tokenService.GerarToken(user);
    }
}