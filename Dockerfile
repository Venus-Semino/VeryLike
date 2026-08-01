# syntax=docker/dockerfile:1
# Build de VeryLike.Web (frontend MVC).
# IMPORTANTE: este Dockerfile se construye con el contexto en la RAÍZ de la
# solución (junto a VeryLike.slnx), porque VeryLike.Web referencia proyectos
# hermanos (Domain, Infrastructure):
#
#   docker build -f VeryLike/Dockerfile -t verylike-web:latest .
#
# ---------- Etapa 1: restaurar y compilar ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos solo los .csproj primero para aprovechar la cache de capas de Docker:
# si el código cambia pero las dependencias no, este paso no se repite.
COPY VeryLike.Domain/VeryLike.Domain.csproj VeryLike.Domain/
COPY VeryLike.Infrastructure/VeryLike.Infrastructure.csproj VeryLike.Infrastructure/
COPY VeryLike/VeryLike.Web.csproj VeryLike/
RUN dotnet restore VeryLike/VeryLike.Web.csproj

# Ahora sí copiamos el resto del código fuente y publicamos.
COPY VeryLike.Domain/ VeryLike.Domain/
COPY VeryLike.Infrastructure/ VeryLike.Infrastructure/
COPY VeryLike/ VeryLike/
RUN dotnet publish VeryLike/VeryLike.Web.csproj -c Release -o /app/publish --no-restore

# ---------- Etapa 2: imagen de ejecución (runtime) ----------
# La imagen final NO trae el SDK completo, solo el runtime de ASP.NET Core:
# resultado mucho más liviano y con menos superficie de ataque.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Ejecuta como usuario sin privilegios (buena práctica para producción/AWS).
USER app

COPY --from=build /app/publish .

# AWS App Runner / ECS enrutan tráfico HTTP a este puerto por convención.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VeryLike.Web.dll"]
