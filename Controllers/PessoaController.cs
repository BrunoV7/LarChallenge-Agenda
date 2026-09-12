using System.Collections;
using Agenda.DTO;
using Agenda.Models;
using Agenda.Services;
using DTO.requests;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Controllers
{
    [ApiController]
    [Route("api/pessoas")]
    public class PessoaController(
        PessoaService service
    ) : ControllerBase
    {

        [HttpGet("find/{cpf}")]
        public async Task<ActionResult<PessoaResponseDTO>> BuscarPorCPF(string cpf)
        {
            return Ok(await service.FindByCPF(cpf));
        }
        [HttpGet("find/all")]
        public async Task<ActionResult<IEnumerable<PessoaResponseDTO>>> GetAllActivePersons()
        {
            return Ok(await service.ListAllPessoas());
        }

        [HttpPost("create")]
        public async Task<ActionResult<PessoaResponseDTO>> Criar([FromBody] PessoaRequestDTO pessoa)
        {
            var criada = await service.CreatePessoa(pessoa);
            return CreatedAtAction(nameof(BuscarPorCPF), new { cpf = criada.CPF }, criada);
        }

        [HttpPut("update/{cpf}")]
        public async Task<ActionResult<PessoaResponseDTO>> AtualizarPessoa([FromBody] PessoaRequestDTO pessoa, string cpf)
        {
            return Ok(await service.UpdatePessoa(cpf, pessoa));
        }

        [HttpDelete("delete/{cpf}")]
        public async Task<IActionResult> DeletarPessoa(string cpf)
        {
            await service.DeletePessoa(cpf);
            return NoContent();
        }
    }
}