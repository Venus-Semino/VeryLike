# VERYLIKE  

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET MVC](https://img.shields.io/badge/ASP.NET%20MVC-0058e6?style=for-the-badge&logo=asp.net&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![AWS](https://img.shields.io/badge/AWS-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)

Ecosistema cinematográfico social (estilo Letterboxd/Instagram) para cinéfilos, organizado como **arquitectura de microservicios en .NET 10** con integración real a TMDB, EF Core y despliegue automatizado a AWS. 

## Arquitectura de la solución

```
VeryLike.slnx
├── VeryLike.Domain            Modelos, interfaces, Factory Method, Strategy (sin dependencias externas)
├── VeryLike.Infrastructure    EF Core (ApplicationDbContext + Repositorios), integración TMDB, hashing de contraseñas
├── VeryLike.Catalog.API       Microservicio REST del catálogo (Swagger, sincronización automática con TMDB)
├── VeryLike.Forum.API         Microservicio REST del foro/comunidad (Swagger)
├── VeryLike                   VeryLike.Web: frontend ASP.NET Core MVC, consume los dos microservicios por HTTP
└── VeryLike.Tests             Pruebas xUnit (AAA) para el Dominio
```

**Quién es dueño de qué dato:**

| Dato | Dueño | Cómo accede `VeryLike.Web` |
|---|---|---|
| Catálogo (Películas/Series) | `VeryLike.Catalog.API` | HTTP, vía `ICatalogoApiClient` |
| Mensajes del Foro | `VeryLike.Forum.API` | HTTP, vía `IForoApiClient` |
| Usuarios / Auth / lista "Para Ver" | `VeryLike.Web` (directo, EF Core) | — |

Los tres servicios comparten la **misma base de datos relacional** (patrón pragmático de "base de datos compartida", no "base de datos por microservicio" — razonable a esta escala; ver los ADR del proyecto para más contexto).

## Cómo correr todo en local

### Opción A: con Docker (recomendada, levanta los 3 servicios + SQL Server de una vez)

```bash
docker compose up --build
```

- Web: <http://localhost:8080>
- Catalog.API (Swagger): <http://localhost:8081/swagger>
- Forum.API (Swagger): <http://localhost:8082/swagger>

Para que la sincronización con TMDB funcione, exporta tu API key antes de levantar los contenedores: 

```bash
export TMDB_API_KEY=tu_api_key_de_tmdb
docker compose up --build
```

### Opción B: sin Docker, desde Visual Studio / CLI

1. Abre `VeryLike.slnx` en Visual Studio 2022 (17.10+) con la carga de trabajo **ASP.NET y desarrollo web**.
2. Configura `ConnectionStrings:DefaultConnection` en los 3 `appsettings.json` (`VeryLike/`, `VeryLike.Catalog.API/`, `VeryLike.Forum.API/`) — deben apuntar a la **misma base de datos**.
3. Aplica las migraciones (basta una vez, desde cualquiera de los tres proyectos que referencian `Infrastructure`):
   ```bash
   dotnet ef migrations add InicialVeryLike -p VeryLike.Infrastructure -s VeryLike.Catalog.API
   dotnet ef database update -p VeryLike.Infrastructure -s VeryLike.Catalog.API
   ```
4. Configura tu API key de TMDB en `VeryLike.Catalog.API` (ahí vive la sincronización automática, no en Web):
   ```bash
   cd VeryLike.Catalog.API
   dotnet user-secrets init
   dotnet user-secrets set "Tmdb:ApiKey" "TU_API_KEY_AQUI"
   ```
5. Marca **los 3 proyectos como "startup projects"** en Visual Studio (clic derecho en la solución → *Configure Startup Projects* → *Multiple startup projects* → `Start` en `VeryLike`, `VeryLike.Catalog.API` y `VeryLike.Forum.API`), o arráncalos por separado con `dotnet run` en cada carpeta.
6. Revisa que `ServiceUrls:CatalogApi` / `ServiceUrls:ForumApi` en `VeryLike/appsettings.json` coincidan con los puertos que asigna `launchSettings.json` de cada microservicio.
7. Desde Swagger de Catalog.API, llama a `POST /api/catalogo/sincronizar` para poblar el catálogo automáticamente desde TMDB.

### Pruebas

```bash
dotnet test VeryLike.Tests/VeryLike.Tests.csproj
```

## Despliegue a AWS

Ver **[`docs/DESPLIEGUE-AWS.md`](docs/DESPLIEGUE-AWS.md)** — guía paso a paso completa (ECR, IAM/OIDC, RDS, App Runner) y `.github/workflows/deploy-aws.yml`, que compila, prueba, construye las 3 imágenes Docker y las despliega automáticamente en cada `push` a `main`.

---

## Registro de cambios de esta revisión

Partiendo del proyecto que subiste (`PROYECTO.zip`), se encontraron y corrigieron los siguientes problemas:

### Errores que impedían compilar
- `VeryLike.Tests.csproj`: rutas de `ProjectReference` duplicadas y mal escritas (`.VeryLike.Catalog.API\...` sin `..\`) → el `dotnet restore` fallaba. Corregido; además se redujeron las referencias a solo `VeryLike.Domain`, que es lo único que las pruebas de Dominio necesitan.
- `VeryLike.Catalog.API.csproj` / `VeryLike.Forum.API.csproj`: referencia duplicada a `..\..\VeryLike.Domain\...` (un nivel de más, ruta inexistente) → corregido. También se quitó la referencia cruzada de `Catalog.API` hacia `Forum.API` (acoplaba dos microservicios que deben ser independientes).
- `VeryLike.Catalog.API/Program.cs`: instanciaba `new CatalogoRepository(rutaCatalogo)` con un `string`, pero el `CatalogoRepository` real solo acepta `ApplicationDbContext` → no compilaba. Reescrito para usar EF Core correctamente.
- `VeryLike.Tests/OrdenarPorCalificacionStrategyTests.cs`: usaba una propiedad `Titulo` (no existe, es `Nombre`) y un método `AplicarEstrategia(...)` (no existe, es `Recomendar(usuario, catalogo)`) → eliminado por inválido y redundante con `MotorDeRecomendacionTests.cs`.
- `VeryLike/Data/Migrations/*.cs`: eran migraciones huérfanas del scaffold original de **ASP.NET Core Identity** (`CreateIdentitySchema`), y referenciaban una clase `VeryLike.Data.ApplicationDbContext` que no existe en ningún lado del proyecto (el `ApplicationDbContext` real vive en `VeryLike.Infrastructure.Data`) → `CS0246` en 3 archivos. Como los proyectos SDK-style compilan todos los `.cs` por defecto, esto por sí solo ya rompía el build de `VeryLike.Web`. Se eliminó toda la carpeta `VeryLike/Data/` (Identity nunca se usó realmente; la autenticación es la propia, con `IUsuarioRepository` + `Sha256PasswordHasher`).

### Compilaba, pero fallaba en tiempo de ejecución
- `Views/Peliculas/Index.cshtml` era una copia exacta de `Views/Pizarron/Index.cshtml` (mismo `@model PizarronViewModel`) → al visitar `/Peliculas` tronaba. Restaurada la vista correcta.
- `appsettings.json` de `VeryLike.Web` seguía siendo el generado por Visual Studio al crear el proyecto, sin la configuración necesaria.

### Arquitectura completada (lo que pedía la nueva especificación)
- `VeryLike.Forum.API` era el scaffold por defecto de Visual Studio (`/weatherforecast`, sin Swagger, sin datos) → implementado por completo: `MensajeForoController`, Swagger, EF Core, CORS configurable.
- `VeryLike.Catalog.API` → se agregó `POST /api/catalogo/sincronizar`, que trae títulos populares de TMDB automáticamente (usa `ContenidoFactory`, deduplica por `IdExterno`).
- `ICatalogoApiClient` existía pero nunca se usaba (código huérfano); `VeryLike.Web` seguía leyendo la base de datos directamente. Ahora `PeliculasController`, `SeriesController` y `PizarronController` consumen `Catalog.API` por HTTP, y se creó `IForoApiClient`/`ForoApiClient` (antes inexistente) para que `ForoController` consuma `Forum.API`.
- Se agregó **Options Pattern** (`ServiceUrlsOptions`) para configurar las URLs de los microservicios por `appsettings.json`/variables de entorno.
- Se agregaron **3 Dockerfiles multi-stage** (uno por servicio desplegable), `docker-compose.yml` para probar todo en local, y `.github/workflows/deploy-aws.yml` (build → test → push a ECR → deploy a App Runner, con alternativa ECS comentada).
- Se corrigió `.github/workflows/ci.yml`, que compilaba `VeryLike.Web.csproj` desde una ruta que no existía en la raíz del repo.
- Se eliminaron los archivos `.json` huérfanos (`catalogo.json`, `usuarios.json`, `mensajesforo.json`) que ya nadie leía.
