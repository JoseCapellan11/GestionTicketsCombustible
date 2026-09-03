using System.Security.Claims;
using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class TanquesController : Controller
{
    private readonly ITanqueService _tanqueService;

    public TanquesController(ITanqueService tanqueService)
    {
        _tanqueService = tanqueService;
    }

    public async Task<IActionResult> Index()
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        return View(tanques.ToList());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearTanqueDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _tanqueService.CrearAsync(dto, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _tanqueService.DesactivarAsync(id, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }
}