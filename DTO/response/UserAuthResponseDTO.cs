using Agenda.Models;

namespace Agenda.DTOs
{
    public record TokenResponse(string Token, DateTime ExpiraEm);

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";

        public UserResponse(User user)
        {
            Id = user.Id;
            Nome = user.Nome;
            Email = user.Email;
            Role = user.Role.ToString();
        }
    }
}