namespace GestionTicketsCombustible.Domain.Entities;

public class Vehiculo
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Ficha { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;
    public decimal CapacidadTanque { get; set; }
    public int Kilometraje { get; set; }
    public bool Activo { get; set; } = true;
}