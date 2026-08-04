using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using VeryLike.Domain.Factories;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Recomendaciones;
using VeryLike.Domain.Servicios;
using VeryLike.Infrastructure.ExternalServices;
using VeryLike.Infrastructure.Data;
using VeryLike.Infrastructure.Repositories;
using VeryLike.Infrastructure.Security;
using VeryLike.Web.Options;
using VeryLike.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. Sesión simple en memoria: guarda qué usuario inició sesión (AuthController).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Persistencia con EF Core: VeryLike.Web SOLO la usa para Usuario/Auth
// (incluida la lista "Para Ver", que referencia contenido por Id sobre la
// misma base de datos compartida). El catálogo y el foro ya NO se leen
// aquí: se piden por HTTP a Catalog.API y Forum.API (ver más abajo).
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
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IMensajeForoRepository, MensajeForoRepository>();
builder.Services.AddSingleton<GeneradorHashtags>();
builder.Services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();

// 4. Patrón Strategy: se registran ambas estrategias concretas; el
// controlador decide cuál usar en tiempo de ejecución (ver PizarronController).
builder.Services.AddScoped<OrdenarPorCalificacionStrategy>();
builder.Services.AddScoped<RecomendacionInteligenteIaStrategy>();

// 5. Options Pattern: URLs de los microservicios, configurables por
// appsettings.json o por variables de entorno (ej. en contenedores/AWS:
// ServiceUrls__CatalogApi, ServiceUrls__ForumApi), sin recompilar.
builder.Services.Configure<ServiceUrlsOptions>(
    builder.Configuration.GetSection(ServiceUrlsOptions.SeccionConfiguracion));

// Options Pattern + cliente tipado para la API externa de cine (TMDB), que
// alimenta el catálogo automáticamente (ver SincronizadorCatalogoService).
builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ContenidoFactory>();
builder.Services.AddScoped<SincronizadorCatalogoService>();
builder.Services.AddHttpClient<ICatalogoExternoService, TmdbCatalogoExternoService>((sp, http) =>
{
    var opciones = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TmdbOptions>>().Value;
    http.BaseAddress = new Uri(opciones.BaseUrl);
});

// 6. Clientes HTTP tipados hacia los microservicios. El BaseAddress se toma
// de ServiceUrlsOptions ya resuelto en el contenedor de DI.
builder.Services.AddHttpClient<ICatalogoApiClient, CatalogoApiClient>((sp, http) =>
{
    var urls = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ServiceUrlsOptions>>().Value;
    http.BaseAddress = new Uri(urls.CatalogApi);
});

builder.Services.AddHttpClient<IForoApiClient, ForoApiClient>((sp, http) =>
{
    var urls = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ServiceUrlsOptions>>().Value;
    http.BaseAddress = new Uri(urls.ForumApi);
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("web");
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHealthChecks("/health");

// En producción solo se habilita temporalmente con
// Database__ApplyMigrations=true durante la primera puesta en marcha.
if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
