using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VeryLike.Domain.Models;

namespace VeryLike.Infrastructure.Data
{
    /// <summary>
    /// Reemplaza por completo la persistencia en peliculas.json / usuarios.json
    /// y los "lock" en memoria: todo el estado vive ahora en una base de
    /// datos relacional administrada por EF Core.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ContenidoAudiovisual> Contenidos => Set<ContenidoAudiovisual>();
        public DbSet<Pelicula> Peliculas => Set<Pelicula>();
        public DbSet<Serie> Series => Set<Serie>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<MensajeForo> MensajesForo => Set<MensajeForo>();
        public DbSet<Calificacion> Calificaciones => Set<Calificacion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurarCatalogo(modelBuilder);
            ConfigurarUsuarios(modelBuilder);
            ConfigurarForo(modelBuilder);
            ConfigurarCalificaciones(modelBuilder);
            SembrarCatalogoDeEjemplo(modelBuilder);
        }

        /// <summary>
        /// Semilla mínima para que el catálogo no esté vacío tras la primera
        /// migración. La sincronización real de contenido llega vía
        /// ICatalogoExternoService (ver TmdbCatalogoExternoService); esto es
        /// solo para no arrancar con la app completamente vacía.
        /// </summary>
        private static void SembrarCatalogoDeEjemplo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pelicula>().HasData(
                new Pelicula
                {
                    Id = 1,
                    Nombre = "Perfect Blue",
                    Genero = new List<string> { "Psicológico", "Animación" },
                    AnioPublicacion = 1997,
                    PlataformaStreaming = "Crunchyroll",
                    Duracion = "1h 21m",
                    Sinopsis = "Una cantante pop convertida en actriz ve cómo su sentido de la realidad se desmorona al ser acosada por un fan obsesivo.",
                    Studio = "Madhouse",
                    Calificacion = 4
                },
                new Pelicula
                {
                    Id = 2,
                    Nombre = "The Imitation Game",
                    Genero = new List<string> { "Drama", "Biografía" },
                    AnioPublicacion = 2014,
                    PlataformaStreaming = "Netflix",
                    Duracion = "1h 54m",
                    Sinopsis = "Alan Turing y su equipo intentan descifrar el código Enigma durante la Segunda Guerra Mundial.",
                    Studio = "Black Bear Pictures",
                    Calificacion = 4.5
                });

            modelBuilder.Entity<Serie>().HasData(
                new Serie
                {
                    Id = 3,
                    Nombre = "Shōgun",
                    Genero = new List<string> { "Drama histórico" },
                    AnioPublicacion = 2024,
                    PlataformaStreaming = "Disney+",
                    Temporadas = 1,
                    Sinopsis = "Un señor feudal japonés y un navegante inglés cambian el rumbo del Japón del siglo XVII.",
                    Studio = "FX Productions",
                    Calificacion = 5
                },
                new Serie
                {
                    Id = 4,
                    Nombre = "Severance",
                    Genero = new List<string> { "Ciencia ficción", "Thriller psicológico" },
                    AnioPublicacion = 2022,
                    PlataformaStreaming = "Apple TV+",
                    Temporadas = 2,
                    Sinopsis = "Empleados que se someten a un procedimiento para separar sus recuerdos laborales de los personales.",
                    Studio = "Fifth Season",
                    Calificacion = 4.5
                });
        }

        private static void ConfigurarCatalogo(ModelBuilder modelBuilder)
        {
            var generoComparer = new ValueComparer<List<string>>(
                (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                v => v.ToList());

            modelBuilder.Entity<ContenidoAudiovisual>(entity =>
            {
                entity.ToTable("Contenidos");
                entity.HasKey(c => c.Id);

                // Los géneros se guardan como texto separado por "|" para no
                // depender de soporte nativo de arreglos (funciona igual en
                // SQL Server y PostgreSQL).
                entity.Property(c => c.Genero)
                    .HasConversion(
                        v => string.Join('|', v),
                        v => v.Length == 0
                            ? new List<string>()
                            : v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(generoComparer);

                entity.HasIndex(c => c.IdExterno);

                entity.HasDiscriminator<string>("TipoDiscriminador")
                    .HasValue<Pelicula>("Pelicula")
                    .HasValue<Serie>("Serie");
            });
        }

        private static void ConfigurarUsuarios(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
                entity.HasIndex(u => u.Correo);

                // Relación muchos-a-muchos unidireccional (Usuario -> ContenidoAudiovisual)
                // con tabla puente explícita "UsuariosParaVer".
                entity.HasMany(u => u.ListaParaVer)
                    .WithMany()
                    .UsingEntity(joinBuilder => joinBuilder.ToTable("UsuariosParaVer"));
            });
        }

        private static void ConfigurarForo(ModelBuilder modelBuilder)
        {
            var hashtagsComparer = new ValueComparer<List<string>>(
                (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                v => v.ToList());

            modelBuilder.Entity<MensajeForo>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FechaPublicacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(m => m.Hashtags)
                    .HasConversion(
                        v => string.Join('|', v),
                        v => v.Length == 0
                            ? new List<string>()
                            : v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(hashtagsComparer);

                // Los comentarios son mensajes hijos del mismo tipo; se borran
                // con su publicación raíz sin ciclos de cascada en SQL Server.
                entity.HasMany(m => m.Comentarios)
                    .WithOne(m => m.MensajePadre)
                    .HasForeignKey(m => m.MensajePadreId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });
        }

        private static void ConfigurarCalificaciones(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => new { c.UsuarioId, c.ContenidoId }).IsUnique();

                entity.HasOne(c => c.Usuario)
                      .WithMany()
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Contenido)
                      .WithMany()
                      .HasForeignKey(c => c.ContenidoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
