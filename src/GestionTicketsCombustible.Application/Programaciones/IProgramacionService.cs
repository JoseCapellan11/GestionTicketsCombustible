namespace GestionTicketsCombustible.Application.Programaciones;

public interface IProgramacionService
{
    Task<IEnumerable<ProgramacionSolicitudDto>> ObtenerTodasAsync();
    Task<int> CrearAsync(CrearProgramacionSolicitudDto dto);
    Task DesactivarAsync(int id);
    Task<int> GenerarSolicitudesDebidasAsync();
}