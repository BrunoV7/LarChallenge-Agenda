using Agenda.DTOs;

namespace Agenda.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> Register(UserRegisterRequest request); 
        Task<TokenResponse> Login(UserLoginRequest request);         
    }
}