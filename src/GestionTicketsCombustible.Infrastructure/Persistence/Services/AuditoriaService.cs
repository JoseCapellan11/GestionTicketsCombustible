using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly ApplicationDbContext _context;

    public AuditoriaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(
        int? usuarioId,
        string nombreUsuario,
        TipoAccionAuditoria tipoAccion,
        string entidad,
        int? entidadId,
        string descripcion,
        string direccionIp)
    {
        var log = new AuditoriaLog
        {
            UsuarioId = usuarioId,
            NombreUsuarioSnapshot = nombreUsuario,
            TipoAccion = tipoAccion,
            Entidad = entidad,
            EntidadId = entidadId,
            Descripcion = descripcion,
            DireccionIp = direccionIp,
            FechaHora = DateTime.Now
        };

        _context.AuditoriaLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditoriaLogDto>> ObtenerTodosAsync()
    {
        return await _context.AuditoriaLogs
            .OrderByDescending(a => a.FechaHora)
            .Take(500)
            .Select(a => new AuditoriaLogDto
            {
                Id = a.Id,
                NombreUsuarioSnapshot = a.NombreUsuarioSnapshot,
                TipoAccion = a.TipoAccion.ToString(),
                Entidad = a.Entidad,
                EntidadId = a.EntidadId,
                Descripcion = a.Descripcion,
                DireccionIp = a.DireccionIp,
                FechaHora = a.FechaHora
            })
            .ToListAsync();
    }
}