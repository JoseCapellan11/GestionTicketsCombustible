using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using GestionTicketsCombustible.Application.Reportes;
using GestionTicketsCombustible.Application.Tickets;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class ReporteService : IReporteService
{
    private readonly ApplicationDbContext _context;
    private readonly ITicketService _ticketService;

    public ReporteService(ApplicationDbContext context, ITicketService ticketService)
    {
        _context = context;
        _ticketService = ticketService;
    }

    public async Task<List<TicketDto>> ObtenerReporteTicketsAsync(ReporteTicketFiltroDto filtro)
    {
        var tickets = (await _ticketService.ObtenerTodosAsync()).AsEnumerable();

        if (filtro.FechaDesde.HasValue)
            tickets = tickets.Where(t => t.FechaCreacion >= filtro.FechaDesde.Value.Date);

        if (filtro.FechaHasta.HasValue)
            tickets = tickets.Where(t => t.FechaCreacion < filtro.FechaHasta.Value.Date.AddDays(1));

        if (filtro.EmpleadoId.HasValue)
            tickets = tickets.Where(t => t.EmpleadoId == filtro.EmpleadoId.Value);

        if (filtro.VehiculoId.HasValue)
            tickets = tickets.Where(t => t.VehiculoId == filtro.VehiculoId.Value);

        if (filtro.DepartamentoId.HasValue)
            tickets = tickets.Where(t => t.DepartamentoId == filtro.DepartamentoId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.TipoCombustible))
            tickets = tickets.Where(t => t.TipoCombustible == filtro.TipoCombustible);

        if (filtro.Estado.HasValue)
            tickets = tickets.Where(t => t.Estado == filtro.Estado.Value);

        return tickets.ToList();
    }

    public async Task<List<string>> ObtenerTiposCombustibleAsync()
    {
        return await _context.Tickets
            .Select(t => t.TipoCombustible)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    public async Task<byte[]> GenerarExcelAsync(ReporteTicketFiltroDto filtro)
    {
        var tickets = await ObtenerReporteTicketsAsync(filtro);

        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Reporte de Tickets");

        string[] encabezados = { "Numero", "Empleado", "Vehiculo", "Departamento", "Cantidad", "Tipo Combustible", "Fecha Creacion", "Estado" };
        for (int col = 0; col < encabezados.Length; col++)
        {
            hoja.Cell(1, col + 1).Value = encabezados[col];
        }

        var filaEncabezado = hoja.Row(1);
        filaEncabezado.Style.Font.Bold = true;
        filaEncabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

        int fila = 2;
        foreach (var t in tickets)
        {
            hoja.Cell(fila, 1).Value = t.NumeroTicket;
            hoja.Cell(fila, 2).Value = t.EmpleadoNombreSnapshot;
            hoja.Cell(fila, 3).Value = t.VehiculoPlacaSnapshot;
            hoja.Cell(fila, 4).Value = t.DepartamentoNombreSnapshot;
            hoja.Cell(fila, 5).Value = (double)t.CantidadAutorizada;
            hoja.Cell(fila, 6).Value = t.TipoCombustible;
            hoja.Cell(fila, 7).Value = t.FechaCreacion;
            hoja.Cell(fila, 7).Style.DateFormat.Format = "dd/MM/yyyy";
            hoja.Cell(fila, 8).Value = t.EstadoVisual;
            fila++;
        }

        hoja.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerarCsvAsync(ReporteTicketFiltroDto filtro)
    {
        var tickets = await ObtenerReporteTicketsAsync(filtro);

        var sb = new StringBuilder();
        sb.AppendLine("Numero,Empleado,Vehiculo,Departamento,Cantidad,Tipo Combustible,Fecha Creacion,Estado");

        foreach (var t in tickets)
        {
            sb.AppendLine(string.Join(",",
                EscaparCsv(t.NumeroTicket),
                EscaparCsv(t.EmpleadoNombreSnapshot),
                EscaparCsv(t.VehiculoPlacaSnapshot),
                EscaparCsv(t.DepartamentoNombreSnapshot),
                t.CantidadAutorizada.ToString(CultureInfo.InvariantCulture),
                EscaparCsv(t.TipoCombustible),
                t.FechaCreacion.ToString("dd/MM/yyyy"),
                EscaparCsv(t.EstadoVisual)));
        }

        var preambulo = Encoding.UTF8.GetPreamble();
        var contenido = Encoding.UTF8.GetBytes(sb.ToString());
        return preambulo.Concat(contenido).ToArray();
    }

    public async Task<byte[]> GenerarPdfAsync(ReporteTicketFiltroDto filtro)
    {
        var tickets = await ObtenerReporteTicketsAsync(filtro);

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(column =>
                {
                    column.Item().Text("Reporte de Tickets").FontSize(18).Bold();
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.5f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Numero");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Empleado");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Vehiculo");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Departamento");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Cantidad");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Tipo");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Fecha");
                        header.Cell().Element(EstiloCeldaEncabezado).Text("Estado");
                    });

                    foreach (var t in tickets)
                    {
                        table.Cell().Element(EstiloCelda).Text(t.NumeroTicket);
                        table.Cell().Element(EstiloCelda).Text(t.EmpleadoNombreSnapshot);
                        table.Cell().Element(EstiloCelda).Text(t.VehiculoPlacaSnapshot);
                        table.Cell().Element(EstiloCelda).Text(t.DepartamentoNombreSnapshot);
                        table.Cell().Element(EstiloCelda).Text(t.CantidadAutorizada.ToString("N2"));
                        table.Cell().Element(EstiloCelda).Text(t.TipoCombustible);
                        table.Cell().Element(EstiloCelda).Text(t.FechaCreacion.ToString("dd/MM/yyyy"));
                        table.Cell().Element(EstiloCelda).Text(t.EstadoVisual);
                    }
                });

                page.Footer().AlignCenter().Text($"Total: {tickets.Count} ticket(s)").FontSize(9);
            });
        });

        return documento.GeneratePdf();
    }

    private static string EscaparCsv(string valor)
    {
        if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n'))
        {
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }
        return valor;
    }

    private static IContainer EstiloCeldaEncabezado(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Padding(5)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Darken1)
            .DefaultTextStyle(x => x.Bold());
    }

    private static IContainer EstiloCelda(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
    }
}