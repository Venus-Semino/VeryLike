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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection debe configurarse mediante una variable de entorno o AWS Secrets Manager.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddHealthChecks();
builder.Services.AddCors(options => options.AddPolicy("web", policy =>
{
    var origins = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
    if (origins.Length > 0)
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddScoped<IMensajeForoRepository, MensajeForoRepository>();

var app = builder.Build();

app.UseCors("web");
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/api/foro"));
app.MapHealthChecks("/health");

app.Run();
