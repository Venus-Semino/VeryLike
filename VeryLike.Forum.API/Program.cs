using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Data;
using VeryLike.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MensajeForo se auto-referencia (comentarios), así que hay que cortar el ciclo
// al serializar en vez de reventar con JsonException.
builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMensajeForoRepository, MensajeForoRepository>();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/api/foro"));

app.Run();
