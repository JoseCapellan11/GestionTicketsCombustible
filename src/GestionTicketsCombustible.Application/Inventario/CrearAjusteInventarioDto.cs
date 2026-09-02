using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Inventario;

public class CrearAjusteInventarioDto
{
    [Required]
    public int TanqueId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El volumen debe ser mayor a cero.")]
    public decimal Volumen { get; set; }

    [Required]
    public bool EsPositivo { get; set; }

    [Required]
    [StringLength(300)]
    public string Referencia { get; set; } = string.Empty;
}