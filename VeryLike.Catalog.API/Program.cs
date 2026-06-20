var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para controladores (API REST)
builder.Services.AddControllers();

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

var app = builder.Build();

// 3. Activar Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VeryLike Catalog API V1");
        c.RoutePrefix = "swagger"; // La documentación vivirá en /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 4. Mapear los endpoints a los controladores
app.MapControllers();

app.Run();