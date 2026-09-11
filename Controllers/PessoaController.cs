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
    public class PessoaController : ControllerBase
    {

        private readonly PessoaService service;
        public PessoaController(PessoaService _service)
        {
            service = _service;
        }

        [HttpGet("find/{cpf}")]
        public async Task<PessoaResponseDTO> GetByCpf(string cpf)
        {   
            try
            {
                return await service.FindByCPF(cpf);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString()); 
                throw;
            }
        }

        [HttpGet("find/all")]
        public async Task<List<PessoaResponseDTO>> GetAllActivePersons()
        {
            try
            {
                return await service.ListAllPessoas();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpPost("create")]
        public async Task<PessoaResponseDTO> CriarPessoa([FromBody] PessoaRequestDTO pessoa)
        {
            try
            {
                return await service.CreatePessoa(pessoa);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpPut("update/{cpf}")]
        public async Task<PessoaResponseDTO> AtualizarPessoa([FromBody] PessoaRequestDTO pessoa, string cpf)
        {
            try
            {
                return await service.UpdatePessoa(cpf, pessoa);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpDelete("delete/{cpf}")]
        public async Task<bool> DeletarPessoa(string cpf)
        {
            try
            {
                return await service.DeletePessoa(cpf);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}