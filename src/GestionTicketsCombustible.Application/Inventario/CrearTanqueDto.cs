using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Inventario;

public class CrearTanqueDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string TipoCombustible { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "La capacidad debe ser mayor a cero.")]
    public decimal Capacidad { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal NivelCritico { get; set; }
}