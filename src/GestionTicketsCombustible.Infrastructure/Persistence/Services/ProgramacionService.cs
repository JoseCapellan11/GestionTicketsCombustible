using GestionTicketsCombustible.Application.Programaciones;
using GestionTicketsCombustible.Application.Solicitudes;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class ProgramacionService : IProgramacionService
{
    private readonly ApplicationDbContext _context;
    private readonly ISolicitudService _solicitudService;

    public ProgramacionService(ApplicationDbContext context, ISolicitudService solicitudService)
    {
        _context = context;
        _solicitudService = solicitudService;
    }

    public async Task<IEnumerable<ProgramacionSolicitudDto>> ObtenerTodasAsync()
    {
        var programaciones = await _context.ProgramacionesSolicitud
            .Include(p => p.Empleado)
            .Include(p => p.Vehiculo)
            .Include(p => p.Departamento)
            .OrderBy(p => p.Id)
            .ToListAsync();

        return programaciones.Select(MapToDto);
    }

    public async Task<int> CrearAsync(CrearProgramacionSolicitudDto dto)
    {
        var programacion = new ProgramacionSolicitud
        {
            EmpleadoId = dto.EmpleadoId,
            VehiculoId = dto.VehiculoId,
            DepartamentoId = dto.DepartamentoId,
            CantidadAutorizada = dto.CantidadAutorizada,
            TipoCombustible = dto.TipoCombustible,
            Frecuencia = dto.Frecuencia,
            Activo = true
        };

        _context.ProgramacionesSolicitud.Add(programacion);
        await _context.SaveChangesAsync();

        return programacion.Id;
    }

    public async Task DesactivarAsync(int id)
    {
        var programacion = await _context.ProgramacionesSolicitud.FindAsync(id)
            ?? throw new InvalidOperationException("La programacion no existe.");

        programacion.Activo = false;
        await _context.SaveChangesAsync();
    }

    public async Task<int> GenerarSolicitudesDebidasAsync()
    {
        var ahora = DateTime.Now;
        var programaciones = await _context.ProgramacionesSolicitud
            .Where(p => p.Activo)
            .ToListAsync();

        var generadas = 0;

        foreach (var programacion in programaciones)
        {
            if (!DebeGenerarse(programacion, ahora)) continue;

            var dto = new CrearSolicitudDto
            {
                EmpleadoId = programacion.EmpleadoId,
                VehiculoId = programacion.VehiculoId,
                DepartamentoId = programacion.DepartamentoId,
                CantidadAutorizada = programacion.CantidadAutorizada,
                TipoCombustible = programacion.TipoCombustible,
                TipoSolicitud = TipoSolicitud.AutomaticaProgramada,
                FechaVencimiento = ahora.AddDays(3)
            };

            // usuarioSolicitanteId = null: la genera el sistema, sin una persona detras.
            await _solicitudService.CrearAsync(dto, usuarioSolicitanteId: null);

            programacion.UltimaGeneracion = ahora;
            generadas++;
        }

        await _context.SaveChangesAsync();
        return generadas;
    }

    private static bool DebeGenerarse(ProgramacionSolicitud programacion, DateTime ahora)
    {
        if (programacion.UltimaGeneracion is null) return true;

        var intervalo = programacion.Frecuencia switch
        {
            FrecuenciaProgramacion.Diaria => TimeSpan.FromDays(1),
            FrecuenciaProgramacion.Semanal => TimeSpan.FromDays(7),
            FrecuenciaProgramacion.Mensual => TimeSpan.FromDays(30),
            _ => TimeSpan.MaxValue
        };

        return (ahora - programacion.UltimaGeneracion.Value) >= intervalo;
    }

    private static ProgramacionSolicitudDto MapToDto(ProgramacionSolicitud p) => new()
    {
        Id = p.Id,
        EmpleadoId = p.EmpleadoId,
        EmpleadoNombre = p.Empleado.NombreCompleto,
        VehiculoId = p.VehiculoId,
        VehiculoPlaca = p.Vehiculo.Placa,
        DepartamentoId = p.DepartamentoId,
        DepartamentoNombre = p.Departamento.Nombre,
        CantidadAutorizada = p.CantidadAutorizada,
        TipoCombustible = p.TipoCombustible,
        Frecuencia = p.Frecuencia,
        UltimaGeneracion = p.UltimaGeneracion,
        Activo = p.Activo
    };
}