using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using GestionTicketsCombustible.Application.Tickets;
using Microsoft.Extensions.Options;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class TicketSeguridadService : ITicketSeguridadService
{
    private readonly TicketSeguridadOptions _opciones;

    public TicketSeguridadService(IOptions<TicketSeguridadOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public string GenerarHash(Guid ticketUid, string numeroTicket, string empleadoNombre, string vehiculoPlaca,
        decimal cantidadCombustible, DateTime fechaEmision, DateTime fechaExpiracion)
    {
        var payload = ConstruirPayload(ticketUid, numeroTicket, empleadoNombre, vehiculoPlaca,
            cantidadCombustible, fechaEmision, fechaExpiracion);

        var claveBytes = Encoding.UTF8.GetBytes(_opciones.ClaveFirma);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(claveBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);

        return Convert.ToHexString(hashBytes);
    }

    public bool VerificarHash(string hashEsperado, Guid ticketUid, string numeroTicket, string empleadoNombre,
        string vehiculoPlaca, decimal cantidadCombustible, DateTime fechaEmision, DateTime fechaExpiracion)
    {
        var hashCalculado = GenerarHash(ticketUid, numeroTicket, empleadoNombre, vehiculoPlaca,
            cantidadCombustible, fechaEmision, fechaExpiracion);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashCalculado),
            Encoding.UTF8.GetBytes(hashEsperado));
    }

    public string GenerarTokenValidacion()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes);
    }

    private static string ConstruirPayload(Guid ticketUid, string numeroTicket, string empleadoNombre,
        string vehiculoPlaca, decimal cantidadCombustible, DateTime fechaEmision, DateTime fechaExpiracion)
    {
        // Formato sin sufijo de zona horaria: el valor de la fecha es el mismo
        // venga de memoria (Kind=Local) o de la base de datos (Kind=Unspecified),
        // asi el hash nunca cambia solo por el origen del objeto DateTime.
        const string formatoFecha = "yyyy-MM-ddTHH:mm:ss.fffffff";

        return string.Join('|',
            ticketUid.ToString(),
            numeroTicket,
            empleadoNombre,
            vehiculoPlaca,
            cantidadCombustible.ToString(CultureInfo.InvariantCulture),
            fechaEmision.ToString(formatoFecha, CultureInfo.InvariantCulture),
            fechaExpiracion.ToString(formatoFecha, CultureInfo.InvariantCulture));
    }
}