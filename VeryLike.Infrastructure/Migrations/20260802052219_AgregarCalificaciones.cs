using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeryLike.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCalificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ContenidoId = table.Column<int>(type: "int", nullable: false),
                    Puntaje = table.Column<double>(type: "float", nullable: false),
                    ResenaPublica = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResenaPrivada = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Contenidos_ContenidoId",
                        column: x => x.ContenidoId,
                        principalTable: "Contenidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ContenidoId",
                table: "Calificaciones",
                column: "ContenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_UsuarioId_ContenidoId",
                table: "Calificaciones",
                columns: new[] { "UsuarioId", "ContenidoId" },
                unique: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calificaciones");
        }
    }
}
