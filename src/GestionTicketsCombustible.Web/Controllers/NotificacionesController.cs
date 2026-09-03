using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Notificaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor},{Roles.Auditor}")]
public class NotificacionesController : Controller
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    public async Task<IActionResult> Index()
    {
        var notificaciones = await _notificacionService.ObtenerRecientesAsync();
        return View(notificaciones);
    }

    [HttpPost]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        await _notificacionService.MarcarComoLeidaAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        await _notificacionService.MarcarTodasComoLeidasAsync();
        return RedirectToAction(nameof(Index));
    }
}