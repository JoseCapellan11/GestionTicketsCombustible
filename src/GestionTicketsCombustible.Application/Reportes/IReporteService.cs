using GestionTicketsCombustible.Application.Tickets;

namespace GestionTicketsCombustible.Application.Reportes;

public interface IReporteService
{
    Task<List<TicketDto>> ObtenerReporteTicketsAsync(ReporteTicketFiltroDto filtro);
    Task<List<string>> ObtenerTiposCombustibleAsync();
    Task<byte[]> GenerarExcelAsync(ReporteTicketFiltroDto filtro);
    Task<byte[]> GenerarCsvAsync(ReporteTicketFiltroDto filtro);
    Task<byte[]> GenerarPdfAsync(ReporteTicketFiltroDto filtro);
}