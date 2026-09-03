using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.CierresDiarios;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class CierreDiarioService : ICierreDiarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public CierreDiarioService(ApplicationDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<CierreDiarioDto>> ObtenerTodosAsync()
    {
        var cierres = await _context.CierresDiarios
            .Include(c => c.Detalles).ThenInclude(d => d.Tanque)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();

        return cierres.Select(MapToDto);
    }

    public async Task<CierreDiarioDto?> ObtenerPorIdAsync(int id)
    {
        var cierre = await _context.CierresDiarios
            .Include(c => c.Detalles).ThenInclude(d => d.Tanque)
            .FirstOrDefaultAsync(c => c.Id == id);

        return cierre is null ? null : MapToDto(cierre);
    }

    public async Task<bool> ExisteCierreHoyAsync()
    {
        var hoy = DateTime.Today;
        return await _context.CierresDiarios.AnyAsync(c => c.Fecha == hoy);
    }

    public async Task<PrepararCierreDiarioDto> PrepararCierreAsync()
    {
        var hoy = DateTime.Today;
        var tanques = await _context.Tanques.Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();

        var resultado = new PrepararCierreDiarioDto { Fecha = hoy };

        foreach (var tanque in tanques)
        {
            var (despachado, recibido) = await CalcularMovimientosDelDiaAsync(tanque.Id, hoy);

            resultado.Tanques.Add(new PrepararCierreDetalleDto
            {
                TanqueId = tanque.Id,
                TanqueNombre = tanque.Nombre,
                ExistenciaTeorica = tanque.ExistenciaActual,
                DespachadoDia = despachado,
                RecibidoDia = recibido
            });
        }

        return resultado;
    }

    public async Task<int> ConfirmarCierreAsync(ConfirmarCierreDiarioDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var hoy = DateTime.Today;

        if (await ExisteCierreHoyAsync())
            throw new InvalidOperationException("Ya existe un cierre diario generado para el dia de hoy.");

        var tanquesActivos = await _context.Tanques.Where(t => t.Activo).ToListAsync();

        if (tanquesActivos.Count == 0)
            throw new InvalidOperationException("No hay tanques activos para incluir en el cierre.");

        var cierre = new CierreDiario
        {
            Fecha = hoy,
            FechaHoraCierre = DateTime.Now,
            UsuarioCierreId = usuarioId,
            UsuarioCierreNombreSnapshot = nombreUsuario,
            Observaciones = dto.Observaciones
        };

        decimal totalDespachado = 0;
        decimal totalRecibido = 0;

        foreach (var tanque in tanquesActivos)
        {
            var lectura = dto.LecturasFisicas.FirstOrDefault(l => l.TanqueId == tanque.Id)
                ?? throw new InvalidOperationException($"Falta la lectura fisica del tanque '{tanque.Nombre}'.");

            var (despachado, recibido) = await CalcularMovimientosDelDiaAsync(tanque.Id, hoy);
            var existenciaTeorica = tanque.ExistenciaActual;

            cierre.Detalles.Add(new CierreDiarioDetalle
            {
                TanqueId = tanque.Id,
                ExistenciaTeorica = existenciaTeorica,
                ExistenciaFisica = lectura.ExistenciaFisica,
                Diferencia = lectura.ExistenciaFisica - existenciaTeorica,
                DespachadoDia = despachado,
                RecibidoDia = recibido
            });

            totalDespachado += despachado;
            totalRecibido += recibido;
        }

        cierre.TotalDespachadoDia = totalDespachado;
        cierre.TotalRecibidoDia = totalRecibido;

        _context.CierresDiarios.Add(cierre);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Creacion,
            "CierreDiario", cierre.Id,
            $"Cierre diario del {hoy:d} generado con {cierre.Detalles.Count} tanque(s)",
            direccionIp);

        return cierre.Id;
    }

    public async Task<byte[]?> GenerarActaPdfAsync(int cierreId)
    {
        var cierre = await _context.CierresDiarios
            .Include(c => c.Detalles).ThenInclude(d => d.Tanque)
            .FirstOrDefaultAsync(c => c.Id == cierreId);

        if (cierre is null) return null;

        using var stream = new MemoryStream();

        Document.Create(documento =>
        {
            documento.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Acta Digital de Cierre Diario").FontSize(18).Bold();
                    col.Item().Text($"Fecha: {cierre.Fecha:d}").FontSize(12).FontColor(Colors.Blue.Medium);
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Cerrado por: {cierre.UsuarioCierreNombreSnapshot}");
                    col.Item().Text($"Fecha/Hora de cierre: {cierre.FechaHoraCierre:g}");
                    col.Item().Text($"Total despachado del dia: {cierre.TotalDespachadoDia:N2} galones");
                    col.Item().Text($"Total recibido del dia: {cierre.TotalRecibidoDia:N2} galones");

                    if (!string.IsNullOrWhiteSpace(cierre.Observaciones))
                        col.Item().Text($"Observaciones: {cierre.Observaciones}");

                    col.Item().PaddingTop(10).Text("Detalle por tanque").FontSize(13).Bold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Tanque");
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Teorica");
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Fisica");
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Diferencia");
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Despachado");
                            header.Cell().Element(EstiloCeldaEncabezado).Text("Recibido");
                        });

                        foreach (var d in cierre.Detalles.OrderBy(d => d.Tanque.Nombre))
                        {
                            table.Cell().Element(EstiloCelda).Text(d.Tanque.Nombre);
                            table.Cell().Element(EstiloCelda).Text(d.ExistenciaTeorica.ToString("N2"));
                            table.Cell().Element(EstiloCelda).Text(d.ExistenciaFisica.ToString("N2"));
                            table.Cell().Element(EstiloCelda).Text(d.Diferencia.ToString("N2"))
                                .FontColor(d.Diferencia != 0 ? Colors.Red.Medium : Colors.Black);
                            table.Cell().Element(EstiloCelda).Text(d.DespachadoDia.ToString("N2"));
                            table.Cell().Element(EstiloCelda).Text(d.RecibidoDia.ToString("N2"));
                        }
                    });
                });

                page.Footer().AlignCenter().Text("Documento generado automaticamente por el sistema. Constituye el acta oficial del cierre diario.").FontSize(8);
            });
        }).GeneratePdf(stream);

        return stream.ToArray();
    }

    private static IContainer EstiloCeldaEncabezado(IContainer container) =>
        container.DefaultTextStyle(x => x.Bold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Darken1);

    private static IContainer EstiloCelda(IContainer container) =>
        container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

    private async Task<(decimal Despachado, decimal Recibido)> CalcularMovimientosDelDiaAsync(int tanqueId, DateTime dia)
    {
        var inicioDia = dia.Date;
        var finDia = inicioDia.AddDays(1);

        var despachado = await _context.MovimientosInventario
            .Where(m => m.TanqueId == tanqueId
                && m.TipoMovimiento == TipoMovimientoInventario.Salida
                && m.FechaMovimiento >= inicioDia && m.FechaMovimiento < finDia)
            .SumAsync(m => (decimal?)m.Volumen) ?? 0;

        var recibido = await _context.MovimientosInventario
            .Where(m => m.TanqueId == tanqueId
                && m.TipoMovimiento == TipoMovimientoInventario.Entrada
                && m.FechaMovimiento >= inicioDia && m.FechaMovimiento < finDia)
            .SumAsync(m => (decimal?)m.Volumen) ?? 0;

        return (despachado, recibido);
    }

    private static CierreDiarioDto MapToDto(CierreDiario c) => new()
    {
        Id = c.Id,
        Fecha = c.Fecha,
        FechaHoraCierre = c.FechaHoraCierre,
        UsuarioCierreNombreSnapshot = c.UsuarioCierreNombreSnapshot,
        TotalDespachadoDia = c.TotalDespachadoDia,
        TotalRecibidoDia = c.TotalRecibidoDia,
        Observaciones = c.Observaciones,
        Detalles = c.Detalles.Select(d => new CierreDiarioDetalleDto
        {
            TanqueId = d.TanqueId,
            TanqueNombre = d.Tanque.Nombre,
            ExistenciaTeorica = d.ExistenciaTeorica,
            ExistenciaFisica = d.ExistenciaFisica,
            Diferencia = d.Diferencia,
            DespachadoDia = d.DespachadoDia,
            RecibidoDia = d.RecibidoDia
        }).ToList()
    };
}