using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Inventario;

public class CrearTransferenciaInventarioDto
{
    [Required]
    public int TanqueOrigenId { get; set; }

    [Required]
    public int TanqueDestinoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El volumen debe ser mayor a cero.")]
    public decimal Volumen { get; set; }

    [StringLength(300)]
    public string? Referencia { get; set; }
}