using System.Text.Json.Serialization;
using Agenda.Models;

namespace DTO.requests
{
    public class TelefoneCreationRequest
    {   
        public TipoTelefone Tipo { get; set; }
        public string Numero { get; set; } = "";
        public Guid IdPessoa { get; set; }
        
    }

    public class TelefoneUpdateRequest
{
    public TipoTelefone? Tipo { get; set; } 
    public string Numero { get; set; } = "";
}
}