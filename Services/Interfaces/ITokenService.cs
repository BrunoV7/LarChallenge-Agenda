using Agenda.DTOs;
using Agenda.Models;

namespace Agenda.Services
{
    public interface ITokenService
    {
        TokenResponse GerarToken(User user);
    }
}