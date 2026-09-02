using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Despachador)]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet("validar/{token}")]
    public async Task<ActionResult<TicketDto>> ValidarPorToken(string token)
    {
        var ticket = await _ticketService.ObtenerPorTokenAsync(token);
        return ticket is null ? NotFound() : Ok(ticket);
    }
}