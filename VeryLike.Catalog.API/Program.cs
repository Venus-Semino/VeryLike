using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Repositorie;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para controladores (API REST)
builder.Services.AddControllers();
// Justo después de builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// 2. Configurar Swagger/OpenAPI (Obligatorio para la rúbrica)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VeryLike Catalog API",
        Version = "v1",
        Description = "Microservicio para la gestión del catálogo de Películas y Series de VeryLike.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Venus Semino",
            Email = "venus@ejemplo.com"
        }
    });
});

// 3. INYECCIÓN DE DEPENDENCIAS
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<ISerieRepository, SerieRepository>();

var app = builder.Build();

// 4. Activar Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VeryLike Catalog API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();