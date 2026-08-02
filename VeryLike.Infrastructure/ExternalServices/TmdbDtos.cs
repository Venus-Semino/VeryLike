using System.Text.Json.Serialization;

namespace VeryLike.Infrastructure.ExternalServices
{
    // Estas clases son un mapeo mínimo del subconjunto de la respuesta de
    // TMDB que realmente usamos; no pretenden cubrir el contrato completo.

    internal class TmdbBusquedaResponse<T>
    {
        [JsonPropertyName("results")]
        public List<T> Results { get; set; } = new();
    }

    internal class TmdbPeliculaResultado
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
        [JsonPropertyName("overview")] public string Overview { get; set; } = string.Empty;
        [JsonPropertyName("release_date")] public string? ReleaseDate { get; set; }
        [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
        [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }
        [JsonPropertyName("genre_ids")] public List<int> GenreIds { get; set; } = new();
        [JsonPropertyName("runtime")] public int? Runtime { get; set; }
    }

    internal class TmdbSerieResultado
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("overview")] public string Overview { get; set; } = string.Empty;
        [JsonPropertyName("first_air_date")] public string? FirstAirDate { get; set; }
        [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
        [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }
        [JsonPropertyName("genre_ids")] public List<int> GenreIds { get; set; } = new();
        [JsonPropertyName("number_of_seasons")] public int? NumberOfSeasons { get; set; }
    }

    internal class TmdbGeneroListaResponse
    {
        [JsonPropertyName("genres")] public List<TmdbGenero> Genres { get; set; } = new();
    }

    internal class TmdbGenero
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    }

    internal class TmdbWatchProvidersResponse
    {
        [JsonPropertyName("results")]
        public Dictionary<string, TmdbWatchProvidersRegion>? Results { get; set; }
    }

    internal class TmdbWatchProvidersRegion
    {
        [JsonPropertyName("flatrate")]
        public List<TmdbProvider>? Flatrate { get; set; }
    }

    internal class TmdbProvider
    {
        [JsonPropertyName("provider_name")] public string ProviderName { get; set; } = string.Empty;
    }
}
