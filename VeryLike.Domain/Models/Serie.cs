namespace VeryLike.Domain.Models
{
    public class Serie : ContenidoAudiovisual
    {
        public override string Tipo => "Serie";

        public int Temporadas { get; set; } = 1;
    }
}
