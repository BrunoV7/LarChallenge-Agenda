using Agenda.DTO;
using Agenda.Models;
using Agenda.Services;
using DTO.requests;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/telefones")]
    public class TelefonesController(TelefoneService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TelefoneResponse>>> BuscarTodos()
        {
            return Ok(await service.ListAllTelefones());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TelefoneResponse>> BuscarPorId(Guid id)
        {
            return Ok(await service.FindById(id));
        }

        [HttpPost]
        public async Task<ActionResult<TelefoneResponse>> Criar([FromBody] TelefoneCreationRequest novo)
        {
            var criado = await service.CreateTelefone(novo);
            return CreatedAtAction(nameof(BuscarPorId), new { id = criado.Id}, criado);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TelefoneResponse>> Atualizar(Guid id, [FromBody] TelefoneUpdateRequest atualizado)
        {
            return Ok(await service.UpdateTelefone(id, atualizado));
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await service.DeleteTelefone(id);
            return NoContent();
        }
    }
}