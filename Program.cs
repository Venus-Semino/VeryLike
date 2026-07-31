using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Factories;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Recomendaciones;
using VeryLike.Infrastructure.Data;
using VeryLike.Infrastructure.ExternalServices;
using VeryLike.Infrastructure.Repositories;
using VeryLike.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. Sesión simple en memoria: guarda qué usuario inició sesión (AuthController).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Persistencia real con Entity Framework Core. Ya NO se usan
// peliculas.json / usuarios.json ni locks en memoria: todo vive en la
// base de datos configurada en "ConnectionStrings:DefaultConnection".
//
//   - SQL Server (por defecto):  options.UseSqlServer(...)
//   - PostgreSQL:                options.UseNpgsql(...)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Repositorios (patrón Repository), 100% asíncronos sobre EF Core.
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IMensajeForoRepository, MensajeForoRepository>();

// 5. Seguridad: hashing de contraseñas (reemplaza la comparación en texto plano).
builder.Services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();

// 6. Patrón Factory Method (capa de Dominio).
builder.Services.AddScoped<ContenidoFactory>();

// 7. Patrón Strategy: se registran ambas estrategias concretas; el
// controlador decide cuál usar en tiempo de ejecución (ver PizarronController).
builder.Services.AddScoped<OrdenarPorCalificacionStrategy>();
builder.Services.AddScoped<RecomendacionInteligenteIaStrategy>();

// 8. Integración con la API externa de cine (TMDB) vía HttpClient tipado.
builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));
builder.Services.AddHttpClient<ICatalogoExternoService, TmdbCatalogoExternoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Aplica migraciones pendientes automáticamente al arrancar (cómodo en
// desarrollo/demo; en producción normalmente se prefiere ejecutar
// `dotnet ef database update` como parte del pipeline de despliegue).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
