using System.Security.Claims;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Application.Empleados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

public class EmpleadosController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IDepartamentoService _departamentoService;

    public EmpleadosController(IEmpleadoService empleadoService, IDepartamentoService departamentoService)
    {
        _empleadoService = empleadoService;
        _departamentoService = departamentoService;
    }

    private async Task CargarDepartamentosAsync(int? seleccionado = null)
    {
        var departamentos = await _departamentoService.ObtenerTodosAsync();
        ViewBag.Departamentos = new SelectList(
            departamentos.Where(d => d.Activo), "Id", "Nombre", seleccionado);
    }

    public async Task<IActionResult> Index()
    {
        return View(await _empleadoService.ObtenerTodosAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarDepartamentosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmpleadoDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarDepartamentosAsync(dto.DepartamentoId);
            return View(dto);
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _empleadoService.CrearAsync(dto, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var empleado = await _empleadoService.ObtenerPorIdAsync(id);
        if (empleado is null) return NotFound();
        await CargarDepartamentosAsync(empleado.DepartamentoId);
        return View(empleado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmpleadoDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarDepartamentosAsync(dto.DepartamentoId);
            return View(dto);
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _empleadoService.ActualizarAsync(dto, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _empleadoService.CambiarEstadoAsync(id, activo, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }
}