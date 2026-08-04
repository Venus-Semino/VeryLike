using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Data;
using VeryLike.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Comparte la misma base que VeryLike.Web: el microservicio es el único que
// debería tocar las tablas del catálogo.
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

builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();

var app = builder.Build();

app.UseCors("web");
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/api/catalogo/peliculas"));
app.MapHealthChecks("/health");

app.Run();
