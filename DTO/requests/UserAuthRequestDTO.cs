namespace Agenda.DTOs
{
    public class UserLoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UserRegisterRequest
    {
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}