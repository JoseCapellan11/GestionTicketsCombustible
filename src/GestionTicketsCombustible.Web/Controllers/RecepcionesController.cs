using System.Security.Claims;
using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class RecepcionesController : Controller
{
    private readonly IRecepcionService _recepcionService;
    private readonly ITanqueService _tanqueService;

    public RecepcionesController(IRecepcionService recepcionService, ITanqueService tanqueService)
    {
        _recepcionService = recepcionService;
        _tanqueService = tanqueService;
    }

    private async Task CargarTanquesAsync(int? seleccionado = null)
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        var tanquesActivos = tanques.Where(t => t.Activo);
        ViewBag.Tanques = new SelectList(tanquesActivos, "Id", "Nombre", seleccionado);
    }

    public async Task<IActionResult> Index()
    {
        var recepciones = await _recepcionService.ObtenerTodasAsync();
        return View(recepciones.OrderByDescending(r => r.Fecha).ToList());
    }

    public async Task<IActionResult> Create()
    {
        await CargarTanquesAsync();
        return View(new CrearRecepcionCombustibleDto { Fecha = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearRecepcionCombustibleDto dto)
    {
        if (!ModelState.IsValid)
        {
            await CargarTanquesAsync(dto.TanqueId);
            return View(dto);
        }

        try
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
            await _recepcionService.RegistrarAsync(dto, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarTanquesAsync(dto.TanqueId);
            return View(dto);
        }
    }
}