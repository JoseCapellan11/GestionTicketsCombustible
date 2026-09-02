using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Despachador)]
public class TanquesController : ControllerBase
{
    private readonly ITanqueService _tanqueService;

    public TanquesController(ITanqueService tanqueService)
    {
        _tanqueService = tanqueService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TanqueDto>>> ObtenerTodos()
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        return Ok(tanques.Where(t => t.Activo));
    }
}