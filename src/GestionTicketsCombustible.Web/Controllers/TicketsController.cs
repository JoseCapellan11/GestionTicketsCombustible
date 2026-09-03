using System.Security.Claims;
using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.ObtenerTodosAsync();
        return View(tickets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _ticketService.ObtenerPorIdAsync(id);
        if (ticket is null) return NotFound();
        return View(ticket);
    }

    public async Task<IActionResult> QrImagen(int id)
    {
        var png = await _ticketService.GenerarImagenQrAsync(id);
        if (png is null) return NotFound();
        return File(png, "image/png");
    }

    public async Task<IActionResult> DescargarPdf(int id)
    {
        var pdf = await _ticketService.GenerarPdfAsync(id);
        if (pdf is null) return NotFound();
        return File(pdf, "application/pdf", $"Ticket-{id}.pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor)]
    public async Task<IActionResult> EnviarCorreo(int id)
    {
        await _ticketService.EnviarPorCorreoAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Supervisor}")]
    public async Task<IActionResult> Anular(int id, string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
        {
            TempData["Error"] = "Debes indicar un motivo para anular el ticket.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _ticketService.AnularAsync(id, motivo, usuarioId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Details), new { id });
    }

    [AllowAnonymous]
    [HttpGet("Tickets/Validar/{token}")]
    public async Task<IActionResult> Validar(string token)
    {
        var ticket = await _ticketService.ObtenerPorTokenAsync(token);
        if (ticket is null) return NotFound();

        ViewBag.Token = token;
        return View(ticket);
    }

    [AllowAnonymous]
    [HttpGet("Tickets/QrImagenPorToken/{token}")]
    public async Task<IActionResult> QrImagenPorToken(string token)
    {
        var png = await _ticketService.GenerarImagenQrPorTokenAsync(token);
        if (png is null) return NotFound();
        return File(png, "image/png");
    }
}