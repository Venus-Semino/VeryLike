namespace VeryLike.Web.Options
{
    /// <summary>
    /// Se enlaza con la sección "ServiceUrls" de appsettings.json (o con las
    /// variables de entorno equivalentes, ej. "ServiceUrls__CatalogApi", que
    /// es como normalmente se inyectan en contenedores/AWS). Mantiene las
    /// direcciones de los microservicios fuera del código, para poder
    /// apuntar a localhost en desarrollo y a las URLs reales de AWS en
    /// producción sin recompilar.
    /// </summary>
    public class ServiceUrlsOptions
    {
        public const string SeccionConfiguracion = "ServiceUrls";

        public string CatalogApi { get; set; } = string.Empty;
        public string ForumApi { get; set; } = string.Empty;
    }
}
