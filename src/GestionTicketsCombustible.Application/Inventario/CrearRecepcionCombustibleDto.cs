using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Inventario;

public class CrearRecepcionCombustibleDto
{
    [Required]
    [StringLength(20)]
    public string Rnc { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string NombreSuplidor { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Factura { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El volumen debe ser mayor a cero.")]
    public decimal VolumenRecibido { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public int TanqueId { get; set; }
}