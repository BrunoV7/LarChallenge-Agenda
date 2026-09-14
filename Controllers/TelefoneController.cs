using Agenda.DTOs;
using Agenda.Models;
using Agenda.Services;
using DTO.requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/telefones")]
    [Authorize]
    public class TelefoneController(TelefoneService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<TelefoneResponse>>> BuscarTodos([FromQuery] int page = 1,[FromQuery] int size = 10,[FromQuery] string? cpf = null,[FromQuery] TipoTelefone? tipo = null,[FromQuery] bool ativo = true,[FromQuery] bool desc = false)
        {
            if (!string.IsNullOrWhiteSpace(cpf))
            {
                return Ok(await service.FindAllTelefonesByCpf(cpf, page, size));
            }

            return Ok(await service.ListAllTelefones(page, size, tipo, ativo, desc));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TelefoneResponse>> BuscarPorId(Guid id)
        {
            return Ok(await service.FindById(id));
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<List<TelefoneResponse>>> BuscarPorTelefone([FromQuery] string telefone)
        {
            return Ok(await service.FindByNumero(telefone));
        }


        [HttpPost]
        public async Task<ActionResult<TelefoneResponse>> Criar([FromBody] TelefoneCreationRequest novo)
        {
            var criado = await service.CreateTelefone(novo);
            return CreatedAtAction(nameof(BuscarPorId), new { id = criado.Id }, criado);
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