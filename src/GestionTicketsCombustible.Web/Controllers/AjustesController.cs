using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class AjustesController : Controller
{
    private readonly IMovimientoInventarioService _movimientoService;
    private readonly ITanqueService _tanqueService;

    public AjustesController(IMovimientoInventarioService movimientoService, ITanqueService tanqueService)
    {
        _movimientoService = movimientoService;
        _tanqueService = tanqueService;
    }

    private async Task CargarTanquesAsync(int? seleccionado = null)
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        var tanquesActivos = tanques.Where(t => t.Activo);
        ViewBag.Tanques = new SelectList(tanquesActivos, "Id", "Nombre", seleccionado);
    }

    public async Task<IActionResult> Create()
    {
        await CargarTanquesAsync();
        return View(new CrearAjusteInventarioDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearAjusteInventarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarTanquesAsync(dto.TanqueId);
            return View(dto);
        }

        try
        {
            await _movimientoService.RegistrarAjusteAsync(dto);
            return RedirectToAction("Index", "Inventario");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarTanquesAsync(dto.TanqueId);
            return View(dto);
        }
    }
}