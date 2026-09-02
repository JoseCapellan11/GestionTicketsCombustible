using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Despachos;

public class CrearDespachoDto
{
    [Required]
    public string TokenTicket { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 10000, ErrorMessage = "Los galones deben ser un valor positivo.")]
    public decimal GalonesDespachados { get; set; }

    [Required]
    [StringLength(100)]
    public string Estacion { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Observaciones { get; set; }
}