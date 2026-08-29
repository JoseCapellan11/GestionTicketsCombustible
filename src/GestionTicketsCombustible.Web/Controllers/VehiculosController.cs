using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Application.Vehiculos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

public class VehiculosController : Controller
{
    private readonly IVehiculoService _vehiculoService;
    private readonly IDepartamentoService _departamentoService;

    public VehiculosController(IVehiculoService vehiculoService, IDepartamentoService departamentoService)
    {
        _vehiculoService = vehiculoService;
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
        return View(await _vehiculoService.ObtenerTodosAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarDepartamentosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehiculoDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarDepartamentosAsync(dto.DepartamentoId);
            return View(dto);
        }
        await _vehiculoService.CrearAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vehiculo = await _vehiculoService.ObtenerPorIdAsync(id);
        if (vehiculo is null) return NotFound();
        await CargarDepartamentosAsync(vehiculo.DepartamentoId);
        return View(vehiculo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VehiculoDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarDepartamentosAsync(dto.DepartamentoId);
            return View(dto);
        }
        await _vehiculoService.ActualizarAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo)
    {
        await _vehiculoService.CambiarEstadoAsync(id, activo);
        return RedirectToAction(nameof(Index));
    }
}