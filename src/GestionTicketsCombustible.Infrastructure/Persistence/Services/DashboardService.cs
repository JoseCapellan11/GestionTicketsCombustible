using GestionTicketsCombustible.Application.Dashboard;
using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Application.Tickets;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ITanqueService _tanqueService;
    private readonly ITicketService _ticketService;

    public DashboardService(ApplicationDbContext context, ITanqueService tanqueService, ITicketService ticketService)
    {
        _context = context;
        _tanqueService = tanqueService;
        _ticketService = ticketService;
    }

    public async Task<DashboardDto> ObtenerDashboardAsync()
    {
        var tanques = await _tanqueService.ObtenerTodosAsync();
        var tickets = await _ticketService.ObtenerTodosAsync();

        var dashboard = new DashboardDto
        {
            InventarioActualTotal = tanques.Where(t => t.Activo).Sum(t => t.ExistenciaActual),
            TicketsActivos = tickets.Count(t => t.EstadoVisual is "Pendiente" or "Proximo a vencer"),
            TicketsVencidos = tickets.Count(t => t.EstadoVisual == "Vencido"),
            CombustibleDespachadoTotal = await _context.Despachos.SumAsync(d => (decimal?)d.GalonesDespachados) ?? 0m
        };

        dashboard.ConsumoPorDepartamento = await _context.Despachos
            .GroupBy(d => d.Ticket.DepartamentoNombreSnapshot)
            .Select(g => new ConsumoAgrupadoDto { Nombre = g.Key, TotalGalones = g.Sum(d => d.GalonesDespachados) })
            .OrderByDescending(x => x.TotalGalones)
            .ToListAsync();

        dashboard.ConsumoPorVehiculo = await _context.Despachos
            .GroupBy(d => d.Ticket.VehiculoPlacaSnapshot)
            .Select(g => new ConsumoAgrupadoDto { Nombre = g.Key, TotalGalones = g.Sum(d => d.GalonesDespachados) })
            .OrderByDescending(x => x.TotalGalones)
            .ToListAsync();

        return dashboard;
    }
}