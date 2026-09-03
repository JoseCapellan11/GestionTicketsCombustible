using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Application.Notificaciones;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;
    private readonly INotificacionService _notificacionService;

    public MovimientoInventarioService(ApplicationDbContext context, IAuditoriaService auditoriaService,
        INotificacionService notificacionService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _notificacionService = notificacionService;
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> ObtenerHistorialAsync()
    {
        var movimientos = await _context.MovimientosInventario
            .Include(m => m.Tanque)
            .Include(m => m.TanqueDestino)
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();

        return movimientos.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> ObtenerHistorialPorTanqueAsync(int tanqueId)
    {
        var movimientos = await _context.MovimientosInventario
            .Include(m => m.Tanque)
            .Include(m => m.TanqueDestino)
            .Where(m => m.TanqueId == tanqueId || m.TanqueDestinoId == tanqueId)
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();

        return movimientos.Select(MapToDto);
    }

    public async Task RegistrarEntradaAsync(int tanqueId, decimal volumen, SubTipoMovimientoInventario subTipo, string? referencia, int? recepcionId = null)
    {
        await AplicarMovimientoAsync(tanqueId, volumen, TipoMovimientoInventario.Entrada, subTipo, referencia, recepcionId: recepcionId);
    }

    public async Task RegistrarSalidaAsync(int tanqueId, decimal volumen, SubTipoMovimientoInventario subTipo, string? referencia, int? despachoId = null)
    {
        await AplicarMovimientoAsync(tanqueId, volumen, TipoMovimientoInventario.Salida, subTipo, referencia, despachoId: despachoId);
    }

    public async Task RegistrarAjusteAsync(CrearAjusteInventarioDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var subTipo = dto.EsPositivo ? SubTipoMovimientoInventario.AjustePositivo : SubTipoMovimientoInventario.AjusteNegativo;

        var movimientoId = await AplicarMovimientoAsync(
            dto.TanqueId, dto.Volumen, TipoMovimientoInventario.Ajuste, subTipo, dto.Referencia, esNegativo: !dto.EsPositivo);

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Ajuste,
            "MovimientoInventario", movimientoId,
            $"Ajuste {(dto.EsPositivo ? "positivo" : "negativo")} de {dto.Volumen} galones en tanque #{dto.TanqueId}: {dto.Referencia}",
            direccionIp);

        await _notificacionService.CrearAsync(
            TipoNotificacion.AjusteInventario,
            $"Ajuste {(dto.EsPositivo ? "positivo" : "negativo")} de {dto.Volumen} galones en el tanque #{dto.TanqueId} realizado por '{nombreUsuario}': {dto.Referencia}",
            "MovimientoInventario", movimientoId);
    }

    public async Task RegistrarTransferenciaAsync(CrearTransferenciaInventarioDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        if (dto.TanqueOrigenId == dto.TanqueDestinoId)
            throw new InvalidOperationException("El tanque de origen y destino no pueden ser el mismo.");

        await using var transaccion = await _context.Database.BeginTransactionAsync();

        var tanqueOrigen = await _context.Tanques
            .FromSqlInterpolated($"SELECT * FROM Tanques WITH (UPDLOCK, HOLDLOCK) WHERE Id = {dto.TanqueOrigenId}")
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("El tanque de origen no existe.");

        var tanqueDestino = await _context.Tanques
            .FromSqlInterpolated($"SELECT * FROM Tanques WITH (UPDLOCK, HOLDLOCK) WHERE Id = {dto.TanqueDestinoId}")
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("El tanque de destino no existe.");

        if (tanqueOrigen.ExistenciaActual < dto.Volumen)
            throw new InvalidOperationException("El tanque de origen no tiene suficiente existencia para esta transferencia.");

        if (tanqueDestino.Capacidad - tanqueDestino.ExistenciaActual < dto.Volumen)
            throw new InvalidOperationException("El tanque de destino no tiene capacidad suficiente para recibir esta transferencia.");

        tanqueOrigen.ExistenciaActual -= dto.Volumen;
        tanqueDestino.ExistenciaActual += dto.Volumen;

        var movimiento = new MovimientoInventario
        {
            TipoMovimiento = TipoMovimientoInventario.Transferencia,
            SubTipo = null,
            TanqueId = tanqueOrigen.Id,
            TanqueDestinoId = tanqueDestino.Id,
            Volumen = dto.Volumen,
            FechaMovimiento = DateTime.Now,
            Referencia = dto.Referencia
        };

        _context.MovimientosInventario.Add(movimiento);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Ajuste,
            "MovimientoInventario", movimiento.Id,
            $"Transferencia de {dto.Volumen} galones del tanque #{dto.TanqueOrigenId} al tanque #{dto.TanqueDestinoId}",
            direccionIp);
    }

    private async Task<int> AplicarMovimientoAsync(
        int tanqueId,
        decimal volumen,
        TipoMovimientoInventario tipo,
        SubTipoMovimientoInventario subTipo,
        string? referencia,
        bool esNegativo = false,
        int? despachoId = null,
        int? recepcionId = null)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        var tanque = await _context.Tanques
            .FromSqlInterpolated($"SELECT * FROM Tanques WITH (UPDLOCK, HOLDLOCK) WHERE Id = {tanqueId}")
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("El tanque no existe.");

        var restaExistencia = tipo == TipoMovimientoInventario.Salida || esNegativo;

        if (restaExistencia && tanque.ExistenciaActual < volumen)
            throw new InvalidOperationException("No hay suficiente existencia en el tanque para esta operacion.");

        if (!restaExistencia && tanque.Capacidad - tanque.ExistenciaActual < volumen)
            throw new InvalidOperationException("El tanque no tiene capacidad suficiente para esta operacion.");

        tanque.ExistenciaActual += restaExistencia ? -volumen : volumen;

        var movimiento = new MovimientoInventario
        {
            TipoMovimiento = tipo,
            SubTipo = subTipo,
            TanqueId = tanqueId,
            Volumen = volumen,
            FechaMovimiento = DateTime.Now,
            Referencia = referencia,
            DespachoId = despachoId,
            RecepcionId = recepcionId
        };

        _context.MovimientosInventario.Add(movimiento);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return movimiento.Id;
    }

    private static MovimientoInventarioDto MapToDto(MovimientoInventario m) => new()
    {
        Id = m.Id,
        TipoMovimiento = m.TipoMovimiento.ToString(),
        SubTipo = m.SubTipo?.ToString(),
        TanqueNombre = m.Tanque.Nombre,
        TanqueDestinoNombre = m.TanqueDestino?.Nombre,
        Volumen = m.Volumen,
        FechaMovimiento = m.FechaMovimiento,
        Referencia = m.Referencia
    };
}