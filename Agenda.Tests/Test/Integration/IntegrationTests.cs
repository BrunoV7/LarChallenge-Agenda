using System.Net;

namespace Agenda.Tests
{
    public class IntegrationTests(AgendaWebAppFactory factory) : IClassFixture<AgendaWebAppFactory>
    {
        [Fact]
        public async Task HealthCheck_RetornaSucesso()
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/health");

            var corpo = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode,
                $"Status: {response.StatusCode}. Corpo: {corpo}");
        }

        [Fact]
        public async Task GetPessoas_SemToken_Retorna401()
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/api/pessoas");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_Retorna401()
        {
            var client = factory.CreateClient();
            var body = new StringContent(
                """{"email":"naoexiste@teste.com","password":"senhaerrada"}""",
                System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/auth/login", body);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPessoas_ComTokenInvalido_Retorna401()
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token.invalido.aqui");
            var response = await client.GetAsync("/api/pessoas");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Register_ComEmailInvalido_Retorna400()
        {
            var client = factory.CreateClient();
            var body = new StringContent(
                """{"nome":"Teste","email":"emailinvalido","password":"12345678"}""",
                System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/auth/register", body);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
    }
}