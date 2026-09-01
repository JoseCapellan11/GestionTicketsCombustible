using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Application.Programaciones;
using GestionTicketsCombustible.Application.Vehiculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = Roles.Supervisor)]
public class ProgramacionesController : Controller
{
    private readonly IProgramacionService _programacionService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IVehiculoService _vehiculoService;
    private readonly IDepartamentoService _departamentoService;

    public ProgramacionesController(IProgramacionService programacionService, IEmpleadoService empleadoService,
        IVehiculoService vehiculoService, IDepartamentoService departamentoService)
    {
        _programacionService = programacionService;
        _empleadoService = empleadoService;
        _vehiculoService = vehiculoService;
        _departamentoService = departamentoService;
    }

    public async Task<IActionResult> Index()
    {
        var programaciones = await _programacionService.ObtenerTodasAsync();
        return View(programaciones);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearProgramacionSolicitudDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(dto);
        }

        await _programacionService.CrearAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _programacionService.DesactivarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarAhora()
    {
        var generadas = await _programacionService.GenerarSolicitudesDebidasAsync();
        TempData["Mensaje"] = $"Se generaron {generadas} solicitud(es) automatica(s).";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        var empleados = await _empleadoService.ObtenerTodosAsync();
        var vehiculos = await _vehiculoService.ObtenerTodosAsync();
        var departamentos = await _departamentoService.ObtenerTodosAsync();

        ViewBag.Empleados = new SelectList(empleados.Where(e => e.Activo), "Id", "NombreCompleto");
        ViewBag.Vehiculos = new SelectList(vehiculos.Where(v => v.Activo), "Id", "Placa");
        ViewBag.Departamentos = new SelectList(departamentos.Where(d => d.Activo), "Id", "Nombre");
    }
}