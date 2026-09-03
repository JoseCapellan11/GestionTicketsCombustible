using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Tickets;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly TicketNumeracionOptions _opciones;
    private readonly ITicketSeguridadService _seguridadService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IEmailService _emailService;
    private readonly AplicacionOptions _aplicacionOpciones;
    private readonly IAuditoriaService _auditoriaService;

    public TicketService(ApplicationDbContext context, IOptions<TicketNumeracionOptions> opciones,
        ITicketSeguridadService seguridadService, IQrCodeService qrCodeService, IEmailService emailService,
        IOptions<AplicacionOptions> aplicacionOpciones, IAuditoriaService auditoriaService)
    {
        _context = context;
        _opciones = opciones.Value;
        _seguridadService = seguridadService;
        _qrCodeService = qrCodeService;
        _emailService = emailService;
        _aplicacionOpciones = aplicacionOpciones.Value;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<TicketDto>> ObtenerTodosAsync()
    {
        var tickets = await _context.Tickets
            .OrderByDescending(t => t.FechaCreacion)
            .ToListAsync();

        return tickets.Select(MapToDto);
    }

    public async Task<TicketDto?> ObtenerPorIdAsync(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<TicketDto?> ObtenerPorTokenAsync(string token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TokenValidacion == token);
        if (ticket is null) return null;

        var dto = MapToDto(ticket);
        dto.HashValido = _seguridadService.VerificarHash(
            ticket.HashSeguridad,
            ticket.TicketUid,
            ticket.NumeroTicket,
            ticket.EmpleadoNombreSnapshot,
            ticket.VehiculoPlacaSnapshot,
            ticket.CantidadAutorizada,
            ticket.FechaCreacion,
            ticket.FechaVencimiento);

        return dto;
    }

    public async Task<TicketDto> GenerarDesdeSolicitudAsync(int solicitudId, int usuarioId, string nombreUsuario, string direccionIp)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        var solicitud = await _context.Solicitudes
            .Include(s => s.Empleado)
            .Include(s => s.Vehiculo)
            .Include(s => s.Departamento)
            .FirstOrDefaultAsync(s => s.Id == solicitudId)
            ?? throw new InvalidOperationException("La solicitud no existe.");

        if (solicitud.Estado != EstadoSolicitud.Aprobada)
            throw new InvalidOperationException("Solo se puede generar un ticket a partir de una solicitud aprobada.");

        var ticketUid = Guid.NewGuid();
        var numeroTicket = await GenerarNumeroTicketAsync();
        var fechaCreacion = DateTime.Now;

        var hash = _seguridadService.GenerarHash(
            ticketUid,
            numeroTicket,
            solicitud.Empleado.NombreCompleto,
            solicitud.Vehiculo.Placa,
            solicitud.CantidadAutorizada,
            fechaCreacion,
            solicitud.FechaVencimiento);

        var tokenValidacion = _seguridadService.GenerarTokenValidacion();

        var ticket = new Ticket
        {
            TicketUid = ticketUid,
            NumeroTicket = numeroTicket,
            SolicitudId = solicitud.Id,
            EmpleadoId = solicitud.EmpleadoId,
            VehiculoId = solicitud.VehiculoId,
            DepartamentoId = solicitud.DepartamentoId,
            EmpleadoNombreSnapshot = solicitud.Empleado.NombreCompleto,
            VehiculoPlacaSnapshot = solicitud.Vehiculo.Placa,
            DepartamentoNombreSnapshot = solicitud.Departamento.Nombre,
            CantidadAutorizada = solicitud.CantidadAutorizada,
            TipoCombustible = solicitud.TipoCombustible,
            FechaCreacion = fechaCreacion,
            FechaVencimiento = solicitud.FechaVencimiento,
            Estado = EstadoTicket.Creado,
            HashSeguridad = hash,
            TokenValidacion = tokenValidacion
        };

        _context.Tickets.Add(ticket);
        solicitud.Estado = EstadoSolicitud.Convertida;

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Creacion,
            "Ticket", ticket.Id,
            $"Emision del ticket '{ticket.NumeroTicket}' para el empleado '{ticket.EmpleadoNombreSnapshot}' (vehiculo {ticket.VehiculoPlacaSnapshot}, {ticket.CantidadAutorizada} galones)",
            direccionIp);

        return MapToDto(ticket);
    }

    public async Task EnviarPorCorreoAsync(int ticketId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Empleado)
            .FirstOrDefaultAsync(t => t.Id == ticketId)
            ?? throw new InvalidOperationException("El ticket no existe.");

        var urlValidacion = $"{_aplicacionOpciones.UrlBase}/Tickets/Validar/{ticket.TokenValidacion}";
        var qrPng = _qrCodeService.GenerarPng(urlValidacion);

        await _emailService.EnviarTicketAsync(MapToDto(ticket), ticket.Empleado.Correo, qrPng, urlValidacion);

        ticket.Estado = EstadoTicket.Enviado;
        await _context.SaveChangesAsync();
    }

    public async Task<byte[]?> GenerarImagenQrAsync(int ticketId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null) return null;

        var urlValidacion = $"{_aplicacionOpciones.UrlBase}/Tickets/Validar/{ticket.TokenValidacion}";
        return _qrCodeService.GenerarPng(urlValidacion);
    }

    public async Task<byte[]?> GenerarImagenQrPorTokenAsync(string token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TokenValidacion == token);
        if (ticket is null) return null;

        var urlValidacion = $"{_aplicacionOpciones.UrlBase}/Tickets/Validar/{ticket.TokenValidacion}";
        return _qrCodeService.GenerarPng(urlValidacion);
    }

    public async Task<byte[]?> GenerarPdfAsync(int ticketId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null) return null;

        var urlValidacion = $"{_aplicacionOpciones.UrlBase}/Tickets/Validar/{ticket.TokenValidacion}";
        var qrPng = _qrCodeService.GenerarPng(urlValidacion);
        var estadoVisual = CalcularEstadoVisual(ticket);

        using var stream = new MemoryStream();

        Document.Create(documento =>
        {
            documento.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Ticket de Combustible Digital").FontSize(18).Bold();
                    col.Item().Text(ticket.NumeroTicket).FontSize(14).FontColor(Colors.Blue.Medium);
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Spacing(6);
                    col.Item().Text($"Empleado: {ticket.EmpleadoNombreSnapshot}");
                    col.Item().Text($"Vehiculo: {ticket.VehiculoPlacaSnapshot}");
                    col.Item().Text($"Departamento: {ticket.DepartamentoNombreSnapshot}");
                    col.Item().Text($"Cantidad autorizada: {ticket.CantidadAutorizada} {ticket.TipoCombustible}");
                    col.Item().Text($"Fecha de creacion: {ticket.FechaCreacion:g}");
                    col.Item().Text($"Fecha de vencimiento: {ticket.FechaVencimiento:g}");
                    col.Item().Text($"Estado: {estadoVisual}");
                    col.Item().Text($"Identificador (UUID): {ticket.TicketUid}").FontSize(8).FontColor(Colors.Grey.Medium);

                    col.Item().PaddingTop(15).AlignCenter().Width(150).Image(qrPng);
                    col.Item().AlignCenter().Text("Escanea este codigo para validar el ticket").FontSize(9).Italic();
                });

                page.Footer().AlignCenter().Text("Este documento es generado automaticamente. No editable ni reutilizable.").FontSize(8);
            });
        }).GeneratePdf(stream);

        return stream.ToArray();
    }

    public async Task AnularAsync(int ticketId, string motivo, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId)
            ?? throw new InvalidOperationException("El ticket no existe.");

        if (ticket.Estado != EstadoTicket.Creado && ticket.Estado != EstadoTicket.Enviado)
            throw new InvalidOperationException("Solo se pueden anular tickets en estado 'Creado' o 'Enviado'.");

        ticket.Estado = EstadoTicket.Anulado;
        ticket.MotivoAnulacion = motivo;
        ticket.FechaAnulacion = DateTime.Now;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Anulacion,
            "Ticket", ticket.Id,
            $"Anulacion del ticket '{ticket.NumeroTicket}': {motivo}",
            direccionIp);
    }

    private async Task<string> GenerarNumeroTicketAsync()
    {
        var anio = DateTime.Now.Year;
        var claveAnio = _opciones.ReinicioAnual ? anio : 0;

        var secuencia = await _context.SecuenciasTicket
            .FromSqlInterpolated($"SELECT * FROM SecuenciasTicket WITH (UPDLOCK, HOLDLOCK) WHERE Anio = {claveAnio}")
            .FirstOrDefaultAsync();

        if (secuencia is null)
        {
            secuencia = new SecuenciaTicket { Anio = claveAnio, UltimoNumero = 0 };
            _context.SecuenciasTicket.Add(secuencia);
        }

        secuencia.UltimoNumero++;
        await _context.SaveChangesAsync();

        return $"{_opciones.Prefijo}-{anio}-{secuencia.UltimoNumero:D6}";
    }

    private string CalcularEstadoVisual(Ticket t)
    {
        if (t.Estado == EstadoTicket.Consumido || t.Estado == EstadoTicket.Anulado)
            return t.Estado.ToString();

        var ahora = DateTime.Now;
        if (t.FechaVencimiento < ahora)
            return "Vencido";

        if (t.FechaVencimiento <= ahora.AddHours(_aplicacionOpciones.HorasProximoAVencer))
            return "Proximo a vencer";

        return "Pendiente";
    }

    private TicketDto MapToDto(Ticket t) => new()
    {
        Id = t.Id,
        TicketUid = t.TicketUid,
        NumeroTicket = t.NumeroTicket,
        SolicitudId = t.SolicitudId,
        EmpleadoId = t.EmpleadoId,
        VehiculoId = t.VehiculoId,
        DepartamentoId = t.DepartamentoId,
        EmpleadoNombreSnapshot = t.EmpleadoNombreSnapshot,
        VehiculoPlacaSnapshot = t.VehiculoPlacaSnapshot,
        DepartamentoNombreSnapshot = t.DepartamentoNombreSnapshot,
        CantidadAutorizada = t.CantidadAutorizada,
        TipoCombustible = t.TipoCombustible,
        FechaCreacion = t.FechaCreacion,
        FechaVencimiento = t.FechaVencimiento,
        Estado = t.Estado,
        EstadoVisual = CalcularEstadoVisual(t),
        MotivoAnulacion = t.MotivoAnulacion,
        FechaAnulacion = t.FechaAnulacion
    };
}