using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Inventario;

public interface IMovimientoInventarioService
{
    Task<IEnumerable<MovimientoInventarioDto>> ObtenerHistorialAsync();
    Task<IEnumerable<MovimientoInventarioDto>> ObtenerHistorialPorTanqueAsync(int tanqueId);

    Task RegistrarEntradaAsync(int tanqueId, decimal volumen, SubTipoMovimientoInventario subTipo, string? referencia, int? recepcionId = null);
    Task RegistrarSalidaAsync(int tanqueId, decimal volumen, SubTipoMovimientoInventario subTipo, string? referencia, int? despachoId = null);
    Task RegistrarAjusteAsync(CrearAjusteInventarioDto dto);
    Task RegistrarTransferenciaAsync(CrearTransferenciaInventarioDto dto);
}