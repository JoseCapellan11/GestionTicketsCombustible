using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Application.Notificaciones;
using GestionTicketsCombustible.Application.Tickets;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.Extensions.Options;

namespace GestionTicketsCombustible.Web.BackgroundServices;

public class AlertasBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AlertasBackgroundService> _logger;
    private readonly TimeSpan _intervalo;

    public AlertasBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AlertasBackgroundService> logger,
        IOptions<NotificacionesOptions> opciones)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _intervalo = TimeSpan.FromMinutes(Math.Max(1, opciones.Value.IntervaloMinutosEscaneo));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EvaluarAlertasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluando alertas automaticas.");
            }

            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    private async Task EvaluarAlertasAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
        var tanqueService = scope.ServiceProvider.GetRequiredService<ITanqueService>();
        var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

        var tickets = await ticketService.ObtenerTodosAsync();

        foreach (var ticket in tickets.Where(t => t.EstadoVisual == "Proximo a vencer"))
        {
            if (!await notificacionService.ExisteNotificacionNoLeidaAsync(TipoNotificacion.TicketProximoAVencer, "Ticket", ticket.Id))
            {
                await notificacionService.CrearAsync(
                    TipoNotificacion.TicketProximoAVencer,
                    $"El ticket '{ticket.NumeroTicket}' vence el {ticket.FechaVencimiento:dd/MM/yyyy HH:mm}.",
                    "Ticket", ticket.Id);
            }
        }

        foreach (var ticket in tickets.Where(t => t.EstadoVisual == "Vencido"))
        {
            if (!await notificacionService.ExisteNotificacionNoLeidaAsync(TipoNotificacion.TicketVencido, "Ticket", ticket.Id))
            {
                await notificacionService.CrearAsync(
                    TipoNotificacion.TicketVencido,
                    $"El ticket '{ticket.NumeroTicket}' vencio el {ticket.FechaVencimiento:dd/MM/yyyy HH:mm} y no fue consumido.",
                    "Ticket", ticket.Id);
            }
        }

        var tanques = await tanqueService.ObtenerTodosAsync();

        foreach (var tanque in tanques.Where(t => t.Activo && t.EnNivelCritico))
        {
            if (!await notificacionService.ExisteNotificacionNoLeidaAsync(TipoNotificacion.InventarioBajo, "Tanque", tanque.Id))
            {
                await notificacionService.CrearAsync(
                    TipoNotificacion.InventarioBajo,
                    $"El tanque '{tanque.Nombre}' esta en nivel critico ({tanque.ExistenciaActual:N2} de {tanque.Capacidad:N2} galones).",
                    "Tanque", tanque.Id);
            }
        }
    }
}