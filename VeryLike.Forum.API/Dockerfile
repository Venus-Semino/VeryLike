# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VeryLike.Domain/VeryLike.Domain.csproj VeryLike.Domain/
COPY VeryLike.Infrastructure/VeryLike.Infrastructure.csproj VeryLike.Infrastructure/
COPY VeryLike.Forum.API/VeryLike.Forum.API.csproj VeryLike.Forum.API/
RUN dotnet restore VeryLike.Forum.API/VeryLike.Forum.API.csproj

COPY VeryLike.Domain/ VeryLike.Domain/
COPY VeryLike.Infrastructure/ VeryLike.Infrastructure/
COPY VeryLike.Forum.API/ VeryLike.Forum.API/
RUN dotnet publish VeryLike.Forum.API/VeryLike.Forum.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "VeryLike.Forum.API.dll"]
