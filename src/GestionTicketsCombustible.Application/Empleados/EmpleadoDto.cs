namespace GestionTicketsCombustible.Application.Empleados;

public class EmpleadoDto
{
    public int Id { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string TelefonoMovil { get; set; } = string.Empty;
    public bool Activo { get; set; }
}