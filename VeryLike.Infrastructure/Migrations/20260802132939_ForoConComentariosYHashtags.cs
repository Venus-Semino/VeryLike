using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

using Microsoft.EntityFrameworkCore.Infrastructure;
using VeryLike.Infrastructure.Data;

namespace VeryLike.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260802132939_ForoConComentariosYHashtags")]
    public partial class ForoConComentariosYHashtags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hashtags",
                table: "MensajesForo",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MensajePadreId",
                table: "MensajesForo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajesForo_MensajePadreId",
                table: "MensajesForo",
                column: "MensajePadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_MensajesForo_MensajesForo_MensajePadreId",
                table: "MensajesForo",
                column: "MensajePadreId",
                principalTable: "MensajesForo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MensajesForo_MensajesForo_MensajePadreId",
                table: "MensajesForo");

            migrationBuilder.DropIndex(
                name: "IX_MensajesForo_MensajePadreId",
                table: "MensajesForo");

            migrationBuilder.DropColumn(
                name: "Hashtags",
                table: "MensajesForo");

            migrationBuilder.DropColumn(
                name: "MensajePadreId",
                table: "MensajesForo");
        }
    }
}
