using Agenda.DTOs;
using Agenda.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var result = await service.Register(request);
            return result is null
                ? Conflict("E-mail já cadastrado.")
                : Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            var result = await service.Login(request);
            return result is null
                ? Unauthorized("Credenciais inválidas.")
                : Ok(result);
        }

    }
}