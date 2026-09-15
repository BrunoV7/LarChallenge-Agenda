using Agenda.Data;
using Agenda.DTOs;
using Agenda.Exceptions;
using Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Services;

public class AuthService(AgendaContext context, ITokenService tokenService, ILogger<AuthService> logger) : IAuthService
{
    public async Task<TokenResponse> Register(UserRegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (await context.User.AnyAsync(u => u.Email == email))
            throw new ConflictException("E-mail já cadastrado.");

        var user = new User
        {
            Nome = request.Nome,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        };

        context.User.Add(user);
        await context.SaveChangesAsync();
        logger.LogInformation("Novo usuário registrado: {UserId}", user.Id);
        return tokenService.GerarToken(user);
    }

    public async Task<TokenResponse> Login(UserLoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await context.User
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Tentativa de login falha: {Email}", email);
            throw new UnauthorizedException("Credenciais inválidas.");  
        }

        logger.LogInformation("Login bem-sucedido: {Email}", email);
        return tokenService.GerarToken(user);
    }
}