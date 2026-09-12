using System.Text.Json.Serialization;
using Agenda.Models;

namespace DTO.requests
{
    public class TelefoneCreationRequest
    {   
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoTelefone Tipo { get; set; }
        public string Numero { get; set; } = null!;
        public Guid IdPessoa { get; set; }
        
    }

    public class TelefoneUpdateRequest
{
    public TipoTelefone? Tipo { get; set; } 
    public string Numero { get; set; } = null!;
}
}