using Agenda.DTOs;
using Agenda.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequest request)
        {
            return Ok(await service.Register(request));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            return Ok(await service.Login(request));
        }

    }
}