using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class MovimientosController : Controller
{
    private readonly IMovimientoInventarioService _movimientoService;

    public MovimientosController(IMovimientoInventarioService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    public async Task<IActionResult> Index()
    {
        var historial = await _movimientoService.ObtenerHistorialAsync();
        return View(historial.OrderByDescending(m => m.FechaMovimiento).ToList());
    }
}