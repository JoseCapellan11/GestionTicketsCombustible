using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicketsCombustible.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Modulo4_Inventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TanqueId",
                table: "Despachos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tanques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoCombustible = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Capacidad = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ExistenciaActual = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    NivelCritico = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecepcionesCombustible",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rnc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreSuplidor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Factura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VolumenRecibido = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TanqueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecepcionesCombustible", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecepcionesCombustible_Tanques_TanqueId",
                        column: x => x.TanqueId,
                        principalTable: "Tanques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SubTipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TanqueId = table.Column<int>(type: "int", nullable: false),
                    TanqueDestinoId = table.Column<int>(type: "int", nullable: true),
                    Volumen = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DespachoId = table.Column<int>(type: "int", nullable: true),
                    RecepcionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Despachos_DespachoId",
                        column: x => x.DespachoId,
                        principalTable: "Despachos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_RecepcionesCombustible_RecepcionId",
                        column: x => x.RecepcionId,
                        principalTable: "RecepcionesCombustible",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Tanques_TanqueDestinoId",
                        column: x => x.TanqueDestinoId,
                        principalTable: "Tanques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Tanques_TanqueId",
                        column: x => x.TanqueId,
                        principalTable: "Tanques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_TanqueId",
                table: "Despachos",
                column: "TanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_DespachoId",
                table: "MovimientosInventario",
                column: "DespachoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_RecepcionId",
                table: "MovimientosInventario",
                column: "RecepcionId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_TanqueDestinoId",
                table: "MovimientosInventario",
                column: "TanqueDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_TanqueId",
                table: "MovimientosInventario",
                column: "TanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_RecepcionesCombustible_TanqueId",
                table: "RecepcionesCombustible",
                column: "TanqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Despachos_Tanques_TanqueId",
                table: "Despachos",
                column: "TanqueId",
                principalTable: "Tanques",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despachos_Tanques_TanqueId",
                table: "Despachos");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "RecepcionesCombustible");

            migrationBuilder.DropTable(
                name: "Tanques");

            migrationBuilder.DropIndex(
                name: "IX_Despachos_TanqueId",
                table: "Despachos");

            migrationBuilder.DropColumn(
                name: "TanqueId",
                table: "Despachos");
        }
    }
}
