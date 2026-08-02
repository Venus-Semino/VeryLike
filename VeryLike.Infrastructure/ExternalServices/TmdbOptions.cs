namespace VeryLike.Infrastructure.ExternalServices
{
    /// <summary>
    /// Se enlaza con la sección "Tmdb" de appsettings.json. Consigue una API
    /// key gratuita en https://www.themoviedb.org/settings/api y colócala en
    /// appsettings.Development.json o en user-secrets (no la subas al repo).
    /// </summary>
    public class TmdbOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.themoviedb.org/3/";
        public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/w500";
        public string Region { get; set; } = "MX";
    }
}