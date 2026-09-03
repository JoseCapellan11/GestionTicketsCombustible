using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicketsCombustible.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Modulo5_CierreDiario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CierresDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraCierre = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCierreId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCierreNombreSnapshot = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TotalDespachadoDia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRecibidoDia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresDiarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CierreDiarioDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CierreDiarioId = table.Column<int>(type: "int", nullable: false),
                    TanqueId = table.Column<int>(type: "int", nullable: false),
                    ExistenciaTeorica = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExistenciaFisica = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Diferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DespachadoDia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecibidoDia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreDiarioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierreDiarioDetalles_CierresDiarios_CierreDiarioId",
                        column: x => x.CierreDiarioId,
                        principalTable: "CierresDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CierreDiarioDetalles_Tanques_TanqueId",
                        column: x => x.TanqueId,
                        principalTable: "Tanques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioDetalles_CierreDiarioId",
                table: "CierreDiarioDetalles",
                column: "CierreDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioDetalles_TanqueId",
                table: "CierreDiarioDetalles",
                column: "TanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_Fecha",
                table: "CierresDiarios",
                column: "Fecha",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CierreDiarioDetalles");

            migrationBuilder.DropTable(
                name: "CierresDiarios");
        }
    }
}
