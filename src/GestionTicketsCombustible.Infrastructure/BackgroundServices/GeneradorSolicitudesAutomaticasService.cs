using GestionTicketsCombustible.Application.Programaciones;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GestionTicketsCombustible.Infrastructure.BackgroundServices;

public class GeneradorSolicitudesAutomaticasService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GeneradorSolicitudesAutomaticasService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(1);

    public GeneradorSolicitudesAutomaticasService(IServiceScopeFactory scopeFactory,
        ILogger<GeneradorSolicitudesAutomaticasService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var programacionService = scope.ServiceProvider.GetRequiredService<IProgramacionService>();

                var generadas = await programacionService.GenerarSolicitudesDebidasAsync();
                if (generadas > 0)
                    _logger.LogInformation("Se generaron {Cantidad} solicitudes automaticas.", generadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando solicitudes automaticas.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}