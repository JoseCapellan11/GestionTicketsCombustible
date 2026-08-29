namespace GestionTicketsCombustible.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;
    public string Cargo { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string TelefonoMovil { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}