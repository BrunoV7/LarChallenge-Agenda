using Agenda.DTOs;
using DTO.requests;
using Agenda.Services;
using Microsoft.AspNetCore.Mvc;
using Agenda.Models;
using Microsoft.AspNetCore.Authorization;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/pessoas")]
    [Authorize]
    public class PessoaController(IPessoaService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<PessoaResponseDTO>>> BuscarTodos(
            [FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? nome = null,
            [FromQuery] bool ativo = true, [FromQuery] bool desc = false)
        {
            return Ok(await service.ListAllPessoas(page, size, nome, ativo, desc));
        }

        [HttpGet("{cpf}")]
        public async Task<ActionResult<PessoaResponseDTO>> BuscarPorCpf(string cpf)
        {
            return Ok(await service.FindByCPF(cpf));
        }

        [HttpPost]
        public async Task<ActionResult<PessoaResponseDTO>> Criar([FromBody] PessoaRequestDTO pessoa)
        {
            var criada = await service.CreatePessoa(pessoa);
            return CreatedAtAction(nameof(BuscarPorCpf), new { cpf = criada.CPF }, criada);
        }

        [HttpPut("{cpf}")]
        public async Task<ActionResult<PessoaResponseDTO>> Atualizar(string cpf, [FromBody] PessoaUpdateRequest pessoa)
        {
            return Ok(await service.UpdatePessoa(cpf, pessoa));
        }

        [HttpPut("reativar/{cpf}")]
        public async Task<ActionResult<PessoaResponseDTO>> Reativar(string cpf)
        {
            return Ok(await service.ReativarPessoa(cpf));
        }

        [HttpDelete("{cpf}")]
        public async Task<IActionResult> Deletar(string cpf)
        {
            await service.DeletePessoa(cpf);
            return NoContent();
        }
    }
}