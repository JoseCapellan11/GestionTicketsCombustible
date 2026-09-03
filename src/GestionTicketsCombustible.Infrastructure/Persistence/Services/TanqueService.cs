using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class TanqueService : ITanqueService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public TanqueService(ApplicationDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<TanqueDto>> ObtenerTodosAsync()
    {
        var tanques = await _context.Tanques.OrderBy(t => t.Nombre).ToListAsync();
        return tanques.Select(MapToDto);
    }

    public async Task<TanqueDto?> ObtenerPorIdAsync(int id)
    {
        var tanque = await _context.Tanques.FirstOrDefaultAsync(t => t.Id == id);
        return tanque is null ? null : MapToDto(tanque);
    }

    public async Task<int> CrearAsync(CrearTanqueDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var tanque = new Tanque
        {
            Nombre = dto.Nombre,
            TipoCombustible = dto.TipoCombustible,
            Capacidad = dto.Capacidad,
            NivelCritico = dto.NivelCritico,
            ExistenciaActual = 0,
            Activo = true
        };

        _context.Tanques.Add(tanque);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Creacion,
            "Tanque", tanque.Id,
            $"Creacion del tanque '{tanque.Nombre}' (capacidad {tanque.Capacidad} galones)",
            direccionIp);

        return tanque.Id;
    }

    public async Task DesactivarAsync(int id, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var tanque = await _context.Tanques.FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new InvalidOperationException("El tanque no existe.");

        tanque.Activo = false;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Modificacion,
            "Tanque", tanque.Id,
            $"Tanque '{tanque.Nombre}' desactivado",
            direccionIp);
    }

    public async Task<IEnumerable<InventarioResumenDto>> ObtenerResumenInventarioAsync()
    {
        var ahora = DateTime.Now;
        var inicioDia = ahora.Date;
        var inicioMes = new DateTime(ahora.Year, ahora.Month, 1);

        var tanques = await _context.Tanques.Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();
        var resumen = new List<InventarioResumenDto>();

        foreach (var tanque in tanques)
        {
            var consumoDiario = await _context.MovimientosInventario
                .Where(m => m.TanqueId == tanque.Id
                    && m.TipoMovimiento == TipoMovimientoInventario.Salida
                    && m.FechaMovimiento >= inicioDia)
                .SumAsync(m => (decimal?)m.Volumen) ?? 0;

            var consumoMensual = await _context.MovimientosInventario
                .Where(m => m.TanqueId == tanque.Id
                    && m.TipoMovimiento == TipoMovimientoInventario.Salida
                    && m.FechaMovimiento >= inicioMes)
                .SumAsync(m => (decimal?)m.Volumen) ?? 0;

            resumen.Add(new InventarioResumenDto
            {
                TanqueId = tanque.Id,
                Nombre = tanque.Nombre,
                TipoCombustible = tanque.TipoCombustible,
                Capacidad = tanque.Capacidad,
                ExistenciaActual = tanque.ExistenciaActual,
                Disponibilidad = tanque.Capacidad - tanque.ExistenciaActual,
                NivelCritico = tanque.NivelCritico,
                EnNivelCritico = tanque.ExistenciaActual <= tanque.NivelCritico,
                ConsumoDiario = consumoDiario,
                ConsumoMensual = consumoMensual
            });
        }

        return resumen;
    }

    private static TanqueDto MapToDto(Tanque t) => new()
    {
        Id = t.Id,
        Nombre = t.Nombre,
        TipoCombustible = t.TipoCombustible,
        Capacidad = t.Capacidad,
        ExistenciaActual = t.ExistenciaActual,
        Disponibilidad = t.Capacidad - t.ExistenciaActual,
        NivelCritico = t.NivelCritico,
        EnNivelCritico = t.ExistenciaActual <= t.NivelCritico,
        Activo = t.Activo
    };
}