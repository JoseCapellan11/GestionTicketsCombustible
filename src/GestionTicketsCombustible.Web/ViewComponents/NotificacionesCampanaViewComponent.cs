using GestionTicketsCombustible.Application.Notificaciones;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.ViewComponents;

public class NotificacionesCampanaViewComponent : ViewComponent
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesCampanaViewComponent(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cantidad = await _notificacionService.ContarNoLeidasAsync();
        return View(cantidad);
    }
}