using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Agenda.Models;

namespace DTO.requests
{
    public class TelefoneCreationRequest
    {
        [Required(ErrorMessage = "O tipo é obrigatório.")]
        public TipoTelefone? Tipo { get; set; } 

        [Required(ErrorMessage = "O número é obrigatório.")]
        public string Numero { get; set; } = "";

        [Required(ErrorMessage = "O IdPessoa é obrigatório.")]
        public Guid IdPessoa { get; set; }
    }

    public class TelefoneUpdateRequest
{
    public TipoTelefone? Tipo { get; set; } 
    public string Numero { get; set; } = "";
}
}