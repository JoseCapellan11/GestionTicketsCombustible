using GestionTicketsCombustible.Application.Despachos;
using GestionTicketsCombustible.Application.Tickets;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class DespachoService : IDespachoService
{
    private readonly ApplicationDbContext _context;
    private readonly ITicketSeguridadService _seguridadService;

    public DespachoService(ApplicationDbContext context, ITicketSeguridadService seguridadService)
    {
        _context = context;
        _seguridadService = seguridadService;
    }

    public async Task<int> RegistrarAsync(CrearDespachoDto dto, int usuarioDespachadorId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Despacho)
            .FirstOrDefaultAsync(t => t.TokenValidacion == dto.TokenTicket)
            ?? throw new InvalidOperationException("El ticket no existe o el codigo QR no es valido.");

        var hashValido = _seguridadService.VerificarHash(
            ticket.HashSeguridad,
            ticket.TicketUid,
            ticket.NumeroTicket,
            ticket.EmpleadoNombreSnapshot,
            ticket.VehiculoPlacaSnapshot,
            ticket.CantidadAutorizada,
            ticket.FechaCreacion,
            ticket.FechaVencimiento);

        if (!hashValido)
            throw new InvalidOperationException("La autenticidad del ticket no pudo ser verificada.");

        if (ticket.Estado == EstadoTicket.Anulado)
            throw new InvalidOperationException("Este ticket fue anulado y no puede despacharse.");

        if (ticket.Estado == EstadoTicket.Consumido || ticket.Despacho is not null)
            throw new InvalidOperationException("Este ticket ya fue despachado.");

        if (ticket.FechaVencimiento < DateTime.Now)
            throw new InvalidOperationException("El ticket esta vencido.");

        if (dto.GalonesDespachados > ticket.CantidadAutorizada)
            throw new InvalidOperationException(
                $"Los galones despachados no pueden exceder la cantidad autorizada ({ticket.CantidadAutorizada}).");

        var despacho = new Despacho
        {
            TicketId = ticket.Id,
            UsuarioDespachadorId = usuarioDespachadorId,
            FechaHoraDespacho = DateTime.Now,
            GalonesDespachados = dto.GalonesDespachados,
            Estacion = dto.Estacion,
            Observaciones = dto.Observaciones
        };

        _context.Despachos.Add(despacho);
        ticket.Estado = EstadoTicket.Consumido;

        await _context.SaveChangesAsync();

        return despacho.Id;
    }

    public async Task<DespachoDto?> ObtenerPorIdAsync(int id)
    {
        var despacho = await _context.Despachos
            .Include(d => d.Ticket)
            .FirstOrDefaultAsync(d => d.Id == id);

        return despacho is null ? null : await MapToDtoAsync(despacho);
    }

    public async Task<IEnumerable<DespachoDto>> ObtenerTodosAsync()
    {
        var despachos = await _context.Despachos
            .Include(d => d.Ticket)
            .OrderByDescending(d => d.FechaHoraDespacho)
            .ToListAsync();

        var resultado = new List<DespachoDto>();
        foreach (var despacho in despachos)
            resultado.Add(await MapToDtoAsync(despacho));

        return resultado;
    }

    private async Task<DespachoDto> MapToDtoAsync(Despacho d)
    {
        var despachador = await _context.Users.FirstOrDefaultAsync(u => u.Id == d.UsuarioDespachadorId);

        return new DespachoDto
        {
            Id = d.Id,
            TicketId = d.TicketId,
            NumeroTicket = d.Ticket.NumeroTicket,
            EmpleadoNombreSnapshot = d.Ticket.EmpleadoNombreSnapshot,
            VehiculoPlacaSnapshot = d.Ticket.VehiculoPlacaSnapshot,
            UsuarioDespachadorId = d.UsuarioDespachadorId,
            NombreDespachador = despachador?.UserName ?? "(desconocido)",
            FechaHoraDespacho = d.FechaHoraDespacho,
            GalonesDespachados = d.GalonesDespachados,
            Estacion = d.Estacion,
            Observaciones = d.Observaciones
        };
    }
}