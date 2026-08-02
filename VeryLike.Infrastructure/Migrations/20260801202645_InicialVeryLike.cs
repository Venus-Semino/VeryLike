using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeryLike.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicialVeryLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contenidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: false),
                    PlataformaStreaming = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sinopsis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Studio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Calificacion = table.Column<double>(type: "float", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdExterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoDiscriminador = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Duracion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temporadas = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesForo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesForo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosParaVer",
                columns: table => new
                {
                    ListaParaVerId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosParaVer", x => new { x.ListaParaVerId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_UsuariosParaVer_Contenidos_ListaParaVerId",
                        column: x => x.ListaParaVerId,
                        principalTable: "Contenidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosParaVer_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Contenidos",
                columns: new[] { "Id", "AnioPublicacion", "Calificacion", "Duracion", "Genero", "IdExterno", "Nombre", "PlataformaStreaming", "PosterUrl", "Sinopsis", "Studio", "TipoDiscriminador" },
                values: new object[,]
                {
                    { 1, 1997, 4.0, "1h 21m", "Psicológico|Animación", null, "Perfect Blue", "Crunchyroll", null, "Una cantante pop convertida en actriz ve cómo su sentido de la realidad se desmorona al ser acosada por un fan obsesivo.", "Madhouse", "Pelicula" },
                    { 2, 2014, 4.5, "1h 54m", "Drama|Biografía", null, "The Imitation Game", "Netflix", null, "Alan Turing y su equipo intentan descifrar el código Enigma durante la Segunda Guerra Mundial.", "Black Bear Pictures", "Pelicula" }
                });

            migrationBuilder.InsertData(
                table: "Contenidos",
                columns: new[] { "Id", "AnioPublicacion", "Calificacion", "Genero", "IdExterno", "Nombre", "PlataformaStreaming", "PosterUrl", "Sinopsis", "Studio", "Temporadas", "TipoDiscriminador" },
                values: new object[,]
                {
                    { 3, 2024, 5.0, "Drama histórico", null, "Shōgun", "Disney+", null, "Un señor feudal japonés y un navegante inglés cambian el rumbo del Japón del siglo XVII.", "FX Productions", 1, "Serie" },
                    { 4, 2022, 4.5, "Ciencia ficción|Thriller psicológico", null, "Severance", "Apple TV+", null, "Empleados que se someten a un procedimiento para separar sus recuerdos laborales de los personales.", "Fifth Season", 2, "Serie" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contenidos_IdExterno",
                table: "Contenidos",
                column: "IdExterno");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosParaVer_UsuarioId",
                table: "UsuariosParaVer",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajesForo");

            migrationBuilder.DropTable(
                name: "UsuariosParaVer");

            migrationBuilder.DropTable(
                name: "Contenidos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
