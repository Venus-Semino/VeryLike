using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Data;
using VeryLike.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Comparte la misma base que VeryLike.Web: el microservicio es el único que
// debería tocar las tablas del catálogo.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/api/catalogo/peliculas"));

app.Run();
