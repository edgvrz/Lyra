using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lyra.Migrations
{
    /// <inheritdoc />
    public partial class v2_AprobacionTiendaImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoAprobacion",
                table: "Tiendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Tiendas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoAprobacion",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Tiendas");
        }
    }
}
