using System.Security.Claims;
using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Despachos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Despachador)]
public class DespachosController : ControllerBase
{
    private readonly IDespachoService _despachoService;

    public DespachosController(IDespachoService despachoService)
    {
        _despachoService = despachoService;
    }

    [HttpPost]
    public async Task<ActionResult> Registrar(CrearDespachoDto dto)
    {
        try
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var despachoId = await _despachoService.RegistrarAsync(dto, usuarioId);
            return Ok(new { id = despachoId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DespachoDto>> ObtenerPorId(int id)
    {
        var despacho = await _despachoService.ObtenerPorIdAsync(id);
        return despacho is null ? NotFound() : Ok(despacho);
    }
}