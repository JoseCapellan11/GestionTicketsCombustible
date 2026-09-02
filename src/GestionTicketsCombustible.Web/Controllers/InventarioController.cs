using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionTicketsCombustible.Application.Inventario;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class InventarioController : Controller
{
    private readonly ITanqueService _tanqueService;

    public InventarioController(ITanqueService tanqueService)
    {
        _tanqueService = tanqueService;
    }

    public async Task<IActionResult> Index()
    {
        var resumen = await _tanqueService.ObtenerResumenInventarioAsync();
        return View(resumen);
    }
}