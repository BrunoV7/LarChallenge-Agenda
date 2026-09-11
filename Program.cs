

using Agenda.Data;
using Agenda.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddScoped<PessoaService>();

builder.Services.AddDbContext<AgendaContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AgendaDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
