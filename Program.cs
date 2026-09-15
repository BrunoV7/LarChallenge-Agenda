using System.Text.Json.Serialization;
using Agenda.Converters;
using Agenda.Data;
using Agenda.Extensions;
using Agenda.Middleware;
using Agenda.Models;
using Agenda.Services;
using Agenda.Validators;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiWithJwt();

builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITelefoneService, TelefoneService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICpfValidator, CpfValidator>();
builder.Services.AddScoped<ITelefoneValidator, TelefoneValidator>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddHealthChecks().AddDbContextCheck<AgendaContext>();

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "A chave JWT (Jwt__Key) não está configurada ou tem menos de 32 caracteres. " +
        "Defina-a no arquivo .env (veja .env.example).");
}

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddDbContext<AgendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AgendaDb")));

var app = builder.Build();

// Cria um admin padrão se não houver nenhum admin ativo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgendaContext>();

    db.Database.Migrate();

    if (!await db.User.AnyAsync(u => u.Role == UserRole.Admin && u.IsActive))
    {
        var adminPadrao = await db.User.FirstOrDefaultAsync(u => u.Email == "admin@agenda.com");
        if (adminPadrao != null)
        {
            adminPadrao.IsActive = true;
            adminPadrao.Role = UserRole.Admin;
            adminPadrao.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        }
        else
        {
            db.User.Add(new User
            {
                Nome = "Administrador",
                Email = "admin@agenda.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            });
        }
        await db.SaveChangesAsync();
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();