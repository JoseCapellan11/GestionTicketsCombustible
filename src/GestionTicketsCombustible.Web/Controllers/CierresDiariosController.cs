using System.Security.Claims;
using GestionTicketsCombustible.Application.CierresDiarios;
using GestionTicketsCombustible.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor},{Roles.Auditor}")]
public class CierresDiariosController : Controller
{
    private readonly ICierreDiarioService _cierreDiarioService;

    public CierresDiariosController(ICierreDiarioService cierreDiarioService)
    {
        _cierreDiarioService = cierreDiarioService;
    }

    public async Task<IActionResult> Index()
    {
        var cierres = await _cierreDiarioService.ObtenerTodosAsync();
        return View(cierres);
    }

    public async Task<IActionResult> Details(int id)
    {
        var cierre = await _cierreDiarioService.ObtenerPorIdAsync(id);
        if (cierre is null) return NotFound();
        return View(cierre);
    }

    public async Task<IActionResult> DescargarActaPdf(int id)
    {
        var pdf = await _cierreDiarioService.GenerarActaPdfAsync(id);
        if (pdf is null) return NotFound();
        return File(pdf, "application/pdf", $"ActaCierre-{id}.pdf");
    }

    [Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor}")]
    public async Task<IActionResult> Preparar()
    {
        if (await _cierreDiarioService.ExisteCierreHoyAsync())
        {
            TempData["Info"] = "Ya existe un cierre generado para el dia de hoy.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = await _cierreDiarioService.PrepararCierreAsync();
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor}")]
    public async Task<IActionResult> Confirmar(ConfirmarCierreDiarioDto dto)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        try
        {
            var cierreId = await _cierreDiarioService.ConfirmarCierreAsync(dto, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
            return RedirectToAction(nameof(Details), new { id = cierreId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Preparar));
        }
    }
}