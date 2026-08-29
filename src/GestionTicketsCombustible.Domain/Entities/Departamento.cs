namespace GestionTicketsCombustible.Domain.Entities;

public class Departamento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}