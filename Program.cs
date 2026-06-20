using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar los servicios esenciales de MVC (Sin RazorPages ni Entity Framework)
builder.Services.AddControllersWithViews();

// 2. Conexión de Capas: Inyección de Dependencias
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<ISerieRepository, SerieRepository>();
builder.Services.AddScoped<IMensajeForoRepository, MensajeForoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

// 3. Configuración del flujo de la aplicación (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 4. Habilitar la carga de CSS y JS de tu wwwroot
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 5. Configurar el enrutador principal de MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();