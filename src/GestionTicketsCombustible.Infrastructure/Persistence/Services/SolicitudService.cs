using GestionTicketsCombustible.Application.Solicitudes;
using GestionTicketsCombustible.Application.Tickets;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class SolicitudService : ISolicitudService
{
    private readonly ApplicationDbContext _context;
    private readonly ITicketService _ticketService;

    public SolicitudService(ApplicationDbContext context, ITicketService ticketService)
    {
        _context = context;
        _ticketService = ticketService;
    }

    public async Task<IEnumerable<SolicitudDto>> ObtenerTodasAsync()
    {
        var solicitudes = await _context.Solicitudes
            .Include(s => s.Empleado)
            .Include(s => s.Vehiculo)
            .Include(s => s.Departamento)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return solicitudes.Select(MapToDto);
    }

    public async Task<SolicitudDto?> ObtenerPorIdAsync(int id)
    {
        var solicitud = await _context.Solicitudes
            .Include(s => s.Empleado)
            .Include(s => s.Vehiculo)
            .Include(s => s.Departamento)
            .FirstOrDefaultAsync(s => s.Id == id);

        return solicitud is null ? null : MapToDto(solicitud);
    }

    public async Task<int> CrearAsync(CrearSolicitudDto dto, int? usuarioSolicitanteId)
    {
        var solicitud = new Solicitud
        {
            EmpleadoId = dto.EmpleadoId,
            VehiculoId = dto.VehiculoId,
            DepartamentoId = dto.DepartamentoId,
            UsuarioSolicitanteId = usuarioSolicitanteId,
            CantidadAutorizada = dto.CantidadAutorizada,
            TipoCombustible = dto.TipoCombustible,
            TipoSolicitud = dto.TipoSolicitud,
            Estado = EstadoSolicitud.Pendiente,
            FechaSolicitud = DateTime.Now,
            FechaVencimiento = dto.FechaVencimiento
        };

        _context.Solicitudes.Add(solicitud);
        await _context.SaveChangesAsync();

        return solicitud.Id;
    }

    public async Task AprobarAsync(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id)
            ?? throw new InvalidOperationException("La solicitud no existe.");

        if (solicitud.Estado != EstadoSolicitud.Pendiente)
            throw new InvalidOperationException("Solo se pueden aprobar solicitudes pendientes.");

        solicitud.Estado = EstadoSolicitud.Aprobada;
        await _context.SaveChangesAsync();

        await _ticketService.GenerarDesdeSolicitudAsync(id);
    }

    public async Task RechazarAsync(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id)
            ?? throw new InvalidOperationException("La solicitud no existe.");

        if (solicitud.Estado != EstadoSolicitud.Pendiente)
            throw new InvalidOperationException("Solo se pueden rechazar solicitudes pendientes.");

        solicitud.Estado = EstadoSolicitud.Rechazada;
        await _context.SaveChangesAsync();
    }

    private static SolicitudDto MapToDto(Solicitud s) => new()
    {
        Id = s.Id,
        EmpleadoId = s.EmpleadoId,
        EmpleadoNombre = s.Empleado.NombreCompleto,
        VehiculoId = s.VehiculoId,
        VehiculoPlaca = s.Vehiculo.Placa,
        DepartamentoId = s.DepartamentoId,
        DepartamentoNombre = s.Departamento.Nombre,
        CantidadAutorizada = s.CantidadAutorizada,
        TipoCombustible = s.TipoCombustible,
        TipoSolicitud = s.TipoSolicitud,
        Estado = s.Estado,
        FechaSolicitud = s.FechaSolicitud,
        FechaVencimiento = s.FechaVencimiento
    };
}