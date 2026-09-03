using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Application.Reportes;
using GestionTicketsCombustible.Application.Vehiculos;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor},{Roles.Auditor}")]
public class ReportesController : Controller
{
    private readonly IReporteService _reporteService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IVehiculoService _vehiculoService;
    private readonly IDepartamentoService _departamentoService;

    public ReportesController(IReporteService reporteService, IEmpleadoService empleadoService,
        IVehiculoService vehiculoService, IDepartamentoService departamentoService)
    {
        _reporteService = reporteService;
        _empleadoService = empleadoService;
        _vehiculoService = vehiculoService;
        _departamentoService = departamentoService;
    }

    public async Task<IActionResult> Index(ReporteTicketFiltroDto filtro)
    {
        await CargarListasAsync(filtro);

        var resultados = await _reporteService.ObtenerReporteTicketsAsync(filtro);
        ViewBag.Filtro = filtro;

        return View(resultados);
    }

    public async Task<IActionResult> ExportarExcel(ReporteTicketFiltroDto filtro)
    {
        var excel = await _reporteService.GenerarExcelAsync(filtro);
        var nombreArchivo = $"Reporte-Tickets-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
    }

    public async Task<IActionResult> ExportarCsv(ReporteTicketFiltroDto filtro)
    {
        var csv = await _reporteService.GenerarCsvAsync(filtro);
        var nombreArchivo = $"Reporte-Tickets-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        return File(csv, "text/csv", nombreArchivo);
    }

        public async Task<IActionResult> ExportarPdf(ReporteTicketFiltroDto filtro)
    {
        var pdf = await _reporteService.GenerarPdfAsync(filtro);
        var nombreArchivo = $"Reporte-Tickets-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
        return File(pdf, "application/pdf", nombreArchivo);
    }

    private async Task CargarListasAsync(ReporteTicketFiltroDto filtro)
    {
        var empleados = await _empleadoService.ObtenerTodosAsync();
        var vehiculos = await _vehiculoService.ObtenerTodosAsync();
        var departamentos = await _departamentoService.ObtenerTodosAsync();
        var tiposCombustible = await _reporteService.ObtenerTiposCombustibleAsync();

        ViewBag.Empleados = new SelectList(empleados, "Id", "NombreCompleto", filtro.EmpleadoId);
        ViewBag.Vehiculos = new SelectList(vehiculos, "Id", "Placa", filtro.VehiculoId);
        ViewBag.Departamentos = new SelectList(departamentos, "Id", "Nombre", filtro.DepartamentoId);
        ViewBag.TiposCombustible = new SelectList(tiposCombustible, filtro.TipoCombustible);
        ViewBag.Estados = new SelectList(Enum.GetValues(typeof(EstadoTicket)), filtro.Estado);
    }
}