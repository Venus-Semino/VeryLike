using VeryLike.Domain.Interfaces;
// Asegúrate de que este namespace coincide con el tuyo (a veces es Repositories con 's')
using VeryLike.Infrastructure.Repositorie;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar soporte para controladores (API REST)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// --- SWAGGER APAGADO TEMPORALMENTE PARA EVITAR ERRORES ---
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//     {
//         Title = "VeryLike Catalog API",
//         Version = "v1"
//     });
// });
// ---------------------------------------------------------

// 3. INYECCIÓN DE DEPENDENCIAS
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<ISerieRepository, SerieRepository>();

var app = builder.Build();

// --- SWAGGER UI APAGADO TEMPORALMENTE ---
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "VeryLike Catalog API V1");
//         c.RoutePrefix = "swagger";
//     });
// }
// ----------------------------------------

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();