using GestionTicketsCombustible.Application.Tickets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _opciones;

    public EmailService(IOptions<SmtpOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public async Task EnviarTicketAsync(TicketDto ticket, string correoDestino, byte[] qrPng, string urlValidacion)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_opciones.RemitenteNombre, _opciones.RemitenteCorreo));
        mensaje.To.Add(MailboxAddress.Parse(correoDestino));
        mensaje.Subject = $"Ticket de Combustible {ticket.NumeroTicket}";

        var cuerpo = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Ticket de Combustible</h2>
                <p><strong>Numero:</strong> {ticket.NumeroTicket}</p>
                <p><strong>Empleado:</strong> {ticket.EmpleadoNombreSnapshot}</p>
                <p><strong>Vehiculo:</strong> {ticket.VehiculoPlacaSnapshot}</p>
                <p><strong>Departamento:</strong> {ticket.DepartamentoNombreSnapshot}</p>
                <p><strong>Cantidad autorizada:</strong> {ticket.CantidadAutorizada} galones de {ticket.TipoCombustible}</p>
                <p><strong>Fecha de vencimiento:</strong> {ticket.FechaVencimiento:g}</p>
                <p>Presenta este codigo QR en la estacion de combustible:</p>
                <img src=""cid:qrticket"" />
                <p>O abre este enlace para ver el estado de tu ticket: <a href=""{urlValidacion}"">{urlValidacion}</a></p>"
        };

        cuerpo.LinkedResources.Add("qr-ticket.png", qrPng).ContentId = "qrticket";
        mensaje.Body = cuerpo.ToMessageBody();

        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(_opciones.Host, _opciones.Puerto,
            _opciones.UsarSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
        await cliente.AuthenticateAsync(_opciones.Usuario, _opciones.Password);
        await cliente.SendAsync(mensaje);
        await cliente.DisconnectAsync(true);
    }
}