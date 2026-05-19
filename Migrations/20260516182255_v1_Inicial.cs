using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lyra.Migrations
{
    /// <inheritdoc />
    public partial class v1_Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_AspNetUsers_UserId",
                table: "Favoritos");

            migrationBuilder.DropIndex(
                name: "IX_Favoritos_UserId",
                table: "Favoritos");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Tiendas",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TonoPielIdeal",
                table: "Prendas",
                newName: "TonosPielCompatibles");

            migrationBuilder.RenameColumn(
                name: "TipoCuerpoIdeal",
                table: "Prendas",
                newName: "TiposCuerpoCompatibles");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Favoritos",
                newName: "FechaAgregado");

            migrationBuilder.AlterColumn<int>(
                name: "TonoPiel",
                table: "UsuariosPerfil",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "TipoCuerpo",
                table: "UsuariosPerfil",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "UsuariosPerfil",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "PerfilCompleto",
                table: "UsuariosPerfil",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Tiendas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "Tiendas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Tiendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Tiendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Tiendas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SitioWeb",
                table: "Tiendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagenUrl",
                table: "Prendas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Categoria",
                table: "Prendas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Prendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Prendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPublicacion",
                table: "Prendas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Prendas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Talla",
                table: "Prendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioPerfilId",
                table: "Favoritos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfil_UserId",
                table: "UsuariosPerfil",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_UserId",
                table: "Tiendas",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioPerfilId_PrendaId",
                table: "Favoritos",
                columns: new[] { "UsuarioPerfilId", "PrendaId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_UsuariosPerfil_UsuarioPerfilId",
                table: "Favoritos",
                column: "UsuarioPerfilId",
                principalTable: "UsuariosPerfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_UsuariosPerfil_UsuarioPerfilId",
                table: "Favoritos");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosPerfil_UserId",
                table: "UsuariosPerfil");

            migrationBuilder.DropIndex(
                name: "IX_Tiendas_UserId",
                table: "Tiendas");

            migrationBuilder.DropIndex(
                name: "IX_Favoritos_UsuarioPerfilId_PrendaId",
                table: "Favoritos");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "UsuariosPerfil");

            migrationBuilder.DropColumn(
                name: "PerfilCompleto",
                table: "UsuariosPerfil");

            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "SitioWeb",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Prendas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Prendas");

            migrationBuilder.DropColumn(
                name: "FechaPublicacion",
                table: "Prendas");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Prendas");

            migrationBuilder.DropColumn(
                name: "Talla",
                table: "Prendas");

            migrationBuilder.DropColumn(
                name: "UsuarioPerfilId",
                table: "Favoritos");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tiendas",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "TonosPielCompatibles",
                table: "Prendas",
                newName: "TonoPielIdeal");

            migrationBuilder.RenameColumn(
                name: "TiposCuerpoCompatibles",
                table: "Prendas",
                newName: "TipoCuerpoIdeal");

            migrationBuilder.RenameColumn(
                name: "FechaAgregado",
                table: "Favoritos",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "TonoPiel",
                table: "UsuariosPerfil",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "TipoCuerpo",
                table: "UsuariosPerfil",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Tiendas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "Tiendas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagenUrl",
                table: "Prendas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Prendas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UserId",
                table: "Favoritos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_AspNetUsers_UserId",
                table: "Favoritos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
