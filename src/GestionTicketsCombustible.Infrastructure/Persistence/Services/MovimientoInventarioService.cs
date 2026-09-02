using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly ApplicationDbContext _context;

    public MovimientoInventarioService(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task RegistrarAjusteAsync(CrearAjusteInventarioDto dto)
    {
        var subTipo = dto.EsPositivo ? SubTipoMovimientoInventario.AjustePositivo : SubTipoMovimientoInventario.AjusteNegativo;

        await AplicarMovimientoAsync(dto.TanqueId, dto.Volumen, TipoMovimientoInventario.Ajuste, subTipo, dto.Referencia, esNegativo: !dto.EsPositivo);
    }

    public async Task RegistrarTransferenciaAsync(CrearTransferenciaInventarioDto dto)
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
    }

    private async Task AplicarMovimientoAsync(
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