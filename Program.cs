

using Agenda.Data;
using Agenda.Middleware;
using Agenda.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddScoped<PessoaService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
      context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);  
    };
});

builder.Services.AddDbContext<AgendaContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AgendaDb")));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
