using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Repositorie;

var builder = WebApplication.CreateBuilder(args);

// 1. Controladores (API REST)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// 2. Swagger/OpenAPI
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

// 3. CORS: VeryLike.Web (puerto 5135/7159) necesita poder llamar a esta API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
        policy.WithOrigins("http://localhost:5135", "https://localhost:7159")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 4. Inyección de dependencias: este microservicio es el ÚNICO dueño de
// catalogo.json. Se le pasa la ruta absoluta a su propia copia del archivo.
var rutaCatalogo = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "catalogo.json");
builder.Services.AddSingleton<ICatalogoRepository>(_ => new CatalogoRepository(rutaCatalogo));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VeryLike Catalog API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("PermitirWeb");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
