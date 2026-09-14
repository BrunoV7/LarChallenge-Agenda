using Agenda.DTOs;
using Agenda.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]  
    public class UsersController(UserService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> ListarTodos()
            => Ok(await service.ListarUsuarios());

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await service.DeletarUsuario(id);
            return NoContent();
        }
    }
}