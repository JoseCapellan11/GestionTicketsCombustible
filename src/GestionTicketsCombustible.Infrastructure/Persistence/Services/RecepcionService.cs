using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class RecepcionService : IRecepcionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMovimientoInventarioService _movimientoService;

    public RecepcionService(ApplicationDbContext context, IMovimientoInventarioService movimientoService)
    {
        _context = context;
        _movimientoService = movimientoService;
    }

    public async Task<int> RegistrarAsync(CrearRecepcionCombustibleDto dto)
    {
        var tanque = await _context.Tanques.FirstOrDefaultAsync(t => t.Id == dto.TanqueId)
            ?? throw new InvalidOperationException("El tanque no existe.");

        var recepcion = new RecepcionCombustible
        {
            Rnc = dto.Rnc,
            NombreSuplidor = dto.NombreSuplidor,
            Factura = dto.Factura,
            VolumenRecibido = dto.VolumenRecibido,
            Fecha = dto.Fecha,
            TanqueId = dto.TanqueId
        };

        _context.RecepcionesCombustible.Add(recepcion);
        await _context.SaveChangesAsync();

        var referencia = $"Recepcion #{recepcion.Id} - Factura {dto.Factura} - {dto.NombreSuplidor}";
        await _movimientoService.RegistrarEntradaAsync(
            dto.TanqueId,
            dto.VolumenRecibido,
            SubTipoMovimientoInventario.RecepcionCombustible,
            referencia,
            recepcionId: recepcion.Id);

        return recepcion.Id;
    }

    public async Task<IEnumerable<RecepcionCombustibleDto>> ObtenerTodasAsync()
    {
        var recepciones = await _context.RecepcionesCombustible
            .Include(r => r.Tanque)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

        return recepciones.Select(r => new RecepcionCombustibleDto
        {
            Id = r.Id,
            Rnc = r.Rnc,
            NombreSuplidor = r.NombreSuplidor,
            Factura = r.Factura,
            VolumenRecibido = r.VolumenRecibido,
            Fecha = r.Fecha,
            TanqueId = r.TanqueId,
            TanqueNombre = r.Tanque.Nombre
        });
    }
}