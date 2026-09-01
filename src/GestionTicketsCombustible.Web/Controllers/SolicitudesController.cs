using System.Security.Claims;
using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Application.Solicitudes;
using GestionTicketsCombustible.Application.Vehiculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize]
public class SolicitudesController : Controller
{
    private readonly ISolicitudService _solicitudService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IVehiculoService _vehiculoService;
    private readonly IDepartamentoService _departamentoService;

    public SolicitudesController(ISolicitudService solicitudService, IEmpleadoService empleadoService,
        IVehiculoService vehiculoService, IDepartamentoService departamentoService)
    {
        _solicitudService = solicitudService;
        _empleadoService = empleadoService;
        _vehiculoService = vehiculoService;
        _departamentoService = departamentoService;
    }

    public async Task<IActionResult> Index()
    {
        var solicitudes = await _solicitudService.ObtenerTodasAsync();
        return View(solicitudes);
    }

    [Authorize(Roles = Roles.Solicitante)]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Solicitante)]
    public async Task<IActionResult> Create(CrearSolicitudDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(dto);
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _solicitudService.CrearAsync(dto, usuarioId);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor)]
    public async Task<IActionResult> Aprobar(int id)
    {
        await _solicitudService.AprobarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor)]
    public async Task<IActionResult> Rechazar(int id)
    {
        await _solicitudService.RechazarAsync(id);
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