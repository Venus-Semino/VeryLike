using VeryLike.Domain.Interfaces;
using VeryLike.Infrastructure.Repositorie;
using VeryLike.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC (sin RazorPages ni Entity Framework: ese scaffold de Identity no se
// estaba usando y era lo que rompía el _Layout en cada página)
builder.Services.AddControllersWithViews();

// 2. Sesión simple en memoria: guarda qué usuario inició sesión (AuthController)
// sin necesitar ASP.NET Identity ni base de datos.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Repositorios JSON locales: Usuario y Foro se quedan en Web por ahora.
var rutaDatos = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data");
builder.Services.AddSingleton<IUsuarioRepository>(_ =>
    new UsuarioRepository(Path.Combine(rutaDatos, "usuarios.json")));
builder.Services.AddSingleton<IMensajeForoRepository>(_ =>
    new MensajeForoRepository(Path.Combine(rutaDatos, "mensajesforo.json")));

// 4. Cliente HTTP hacia VeryLike.Catalog.API: el catálogo (películas/series)
// ya NO se lee de un archivo local, se consume vía la API.
builder.Services.AddHttpClient<ICatalogoApiClient, CatalogoApiClient>(client =>
{
    var baseUrl = builder.Configuration["CatalogApi:BaseUrl"] ?? "http://localhost:5033";
    client.BaseAddress = new Uri(baseUrl);
});

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

app.Run();
