namespace GestionTicketsCombustible.Application.Dashboard;

public class DashboardDto
{
    public decimal InventarioActualTotal { get; set; }
    public decimal CombustibleDespachadoTotal { get; set; }
    public int TicketsActivos { get; set; }
    public int TicketsVencidos { get; set; }
    public List<ConsumoAgrupadoDto> ConsumoPorDepartamento { get; set; } = new();
    public List<ConsumoAgrupadoDto> ConsumoPorVehiculo { get; set; } = new();
}