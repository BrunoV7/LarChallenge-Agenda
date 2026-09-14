using System.Text.Json.Serialization;
using Agenda.Converters;
using Agenda.Data;
using Agenda.Extensions;
using Agenda.Middleware;
using Agenda.Services;
using Agenda.Validators;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiWithJwt();

builder.Services.AddScoped<PessoaService>();
builder.Services.AddScoped<TelefoneService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CpfValidator>();
builder.Services.AddScoped<TelefoneValidator>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddHealthChecks().AddDbContextCheck<AgendaContext>();

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();
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