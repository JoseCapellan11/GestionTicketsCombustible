using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicketsCombustible.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Modulo2_ProgramacionesYSolicitudAutomatica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSolicitanteId",
                table: "Solicitudes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ProgramacionesSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    CantidadAutorizada = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TipoCombustible = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Frecuencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UltimaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesSolicitud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramacionesSolicitud_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesSolicitud_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesSolicitud_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesSolicitud_DepartamentoId",
                table: "ProgramacionesSolicitud",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesSolicitud_EmpleadoId",
                table: "ProgramacionesSolicitud",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesSolicitud_VehiculoId",
                table: "ProgramacionesSolicitud",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramacionesSolicitud");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSolicitanteId",
                table: "Solicitudes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
