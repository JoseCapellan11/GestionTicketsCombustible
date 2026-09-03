namespace GestionTicketsCombustible.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardDto> ObtenerDashboardAsync();
}