using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Agenda.Tests
{
    public class AgendaWebAppFactory : WebApplicationFactory<Program>
    {
        public AgendaWebAppFactory()
        {
            Environment.SetEnvironmentVariable("Jwt__Key", "chave-de-teste-com-pelo-menos-32-caracteres-aqui");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "AgendaAPI");
            Environment.SetEnvironmentVariable("Jwt__Audience", "AgendaAPIUsers");
            Environment.SetEnvironmentVariable("Jwt__ExpireMinutes", "60");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}