using GestionTicketsCombustible.Application.Notificaciones;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class NotificacionService : INotificacionService
{
    private readonly ApplicationDbContext _context;

    public NotificacionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificacionDto>> ObtenerRecientesAsync(int cantidad = 50)
    {
        var notificaciones = await _context.Notificaciones
            .OrderByDescending(n => n.FechaCreacion)
            .Take(cantidad)
            .ToListAsync();

        return notificaciones.Select(MapToDto).ToList();
    }

    public async Task<int> ContarNoLeidasAsync()
    {
        return await _context.Notificaciones.CountAsync(n => !n.Leida);
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        var notificacion = await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
        if (notificacion is null || notificacion.Leida) return;

        notificacion.Leida = true;
        notificacion.FechaLectura = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task MarcarTodasComoLeidasAsync()
    {
        var noLeidas = await _context.Notificaciones.Where(n => !n.Leida).ToListAsync();
        var ahora = DateTime.Now;
        foreach (var n in noLeidas)
        {
            n.Leida = true;
            n.FechaLectura = ahora;
        }
        await _context.SaveChangesAsync();
    }

    public async Task CrearAsync(TipoNotificacion tipo, string mensaje, string? entidadRelacionada = null, int? entidadId = null)
    {
        var notificacion = new Notificacion
        {
            Tipo = tipo,
            Mensaje = mensaje,
            EntidadRelacionada = entidadRelacionada,
            EntidadId = entidadId,
            FechaCreacion = DateTime.Now,
            Leida = false
        };

        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteNotificacionNoLeidaAsync(TipoNotificacion tipo, string entidadRelacionada, int entidadId)
    {
        return await _context.Notificaciones.AnyAsync(n =>
            n.Tipo == tipo &&
            n.EntidadRelacionada == entidadRelacionada &&
            n.EntidadId == entidadId &&
            !n.Leida);
    }

    private static NotificacionDto MapToDto(Notificacion n) => new()
    {
        Id = n.Id,
        Tipo = n.Tipo.ToString(),
        Mensaje = n.Mensaje,
        EntidadRelacionada = n.EntidadRelacionada,
        EntidadId = n.EntidadId,
        FechaCreacion = n.FechaCreacion,
        Leida = n.Leida
    };
}