using System.ComponentModel.DataAnnotations;

namespace DTO.requests
{
    public class PessoaUpdateRequest
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string? Nome { get; set; }

        public DateOnly? DataNascimento { get; set; }
    }
}