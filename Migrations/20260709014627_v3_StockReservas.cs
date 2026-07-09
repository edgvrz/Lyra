using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lyra.Migrations
{
    /// <inheritdoc />
    public partial class v3_StockReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Prendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrendaId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioPerfilId = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    EsPresencial = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotasCliente = table.Column<string>(type: "TEXT", nullable: true),
                    FechaReserva = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Prendas_PrendaId",
                        column: x => x.PrendaId,
                        principalTable: "Prendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_UsuariosPerfil_UsuarioPerfilId",
                        column: x => x.UsuarioPerfilId,
                        principalTable: "UsuariosPerfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PrendaId",
                table: "Reservas",
                column: "PrendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioPerfilId_PrendaId",
                table: "Reservas",
                columns: new[] { "UsuarioPerfilId", "PrendaId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Prendas");
        }
    }
}
