using GestionTicketsCombustible.Application.Tickets;
using QRCoder;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class QrCodeService : IQrCodeService
{
    public byte[] GenerarPng(string contenido)
    {
        using var generador = new QRCodeGenerator();
        using var datos = generador.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(datos);
        return pngQr.GetGraphic(20);
    }
}