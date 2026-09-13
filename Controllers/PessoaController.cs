using Agenda.DTO;
using DTO.requests;
using Agenda.Services;
using Microsoft.AspNetCore.Mvc;
using Agenda.Models;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/pessoas")]
    public class PessoaController(PessoaService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<PessoaResponseDTO>>> BuscarTodos([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            return Ok(await service.ListAllPessoas(page, size));
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
        public async Task<ActionResult<PessoaResponseDTO>> Atualizar(string cpf, [FromBody] PessoaRequestDTO pessoa)
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