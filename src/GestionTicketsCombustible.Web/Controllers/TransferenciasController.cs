using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class TransferenciasController : Controller
{
    private readonly IMovimientoInventarioService _movimientoService;
    private readonly ITanqueService _tanqueService;

    public TransferenciasController(IMovimientoInventarioService movimientoService, ITanqueService tanqueService)
    {
        _movimientoService = movimientoService;
        _tanqueService = tanqueService;
    }

    private async Task CargarTanquesAsync(int? origenSeleccionado = null, int? destinoSeleccionado = null)
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        var tanquesActivos = tanques.Where(t => t.Activo).ToList();
        ViewBag.TanquesOrigen = new SelectList(tanquesActivos, "Id", "Nombre", origenSeleccionado);
        ViewBag.TanquesDestino = new SelectList(tanquesActivos, "Id", "Nombre", destinoSeleccionado);
    }

    public async Task<IActionResult> Create()
    {
        await CargarTanquesAsync();
        return View(new CrearTransferenciaInventarioDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearTransferenciaInventarioDto dto)
    {
        if (dto.TanqueOrigenId == dto.TanqueDestinoId)
        {
            ModelState.AddModelError(string.Empty, "El tanque de origen y el de destino no pueden ser el mismo.");
        }

        if (!ModelState.IsValid)
        {
            await CargarTanquesAsync(dto.TanqueOrigenId, dto.TanqueDestinoId);
            return View(dto);
        }

        try
        {
            await _movimientoService.RegistrarTransferenciaAsync(dto);
            return RedirectToAction("Index", "Inventario");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarTanquesAsync(dto.TanqueOrigenId, dto.TanqueDestinoId);
            return View(dto);
        }
    }
}