using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public EmpleadoService(ApplicationDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Select(e => new EmpleadoDto
            {
                Id = e.Id,
                CodigoEmpleado = e.CodigoEmpleado,
                NombreCompleto = e.NombreCompleto,
                Cedula = e.Cedula,
                DepartamentoId = e.DepartamentoId,
                DepartamentoNombre = e.Departamento.Nombre,
                Cargo = e.Cargo,
                Correo = e.Correo,
                TelefonoMovil = e.TelefonoMovil,
                Activo = e.Activo
            })
            .ToListAsync();
    }

    public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
    {
        var e = await _context.Empleados.Include(e => e.Departamento).FirstOrDefaultAsync(e => e.Id == id);
        if (e is null) return null;
        return new EmpleadoDto
        {
            Id = e.Id,
            CodigoEmpleado = e.CodigoEmpleado,
            NombreCompleto = e.NombreCompleto,
            Cedula = e.Cedula,
            DepartamentoId = e.DepartamentoId,
            DepartamentoNombre = e.Departamento.Nombre,
            Cargo = e.Cargo,
            Correo = e.Correo,
            TelefonoMovil = e.TelefonoMovil,
            Activo = e.Activo
        };
    }

    public async Task<int> CrearAsync(EmpleadoDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var empleado = new Empleado
        {
            CodigoEmpleado = dto.CodigoEmpleado,
            NombreCompleto = dto.NombreCompleto,
            Cedula = dto.Cedula,
            DepartamentoId = dto.DepartamentoId,
            Cargo = dto.Cargo,
            Correo = dto.Correo,
            TelefonoMovil = dto.TelefonoMovil,
            Activo = true
        };
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Creacion,
            "Empleado", empleado.Id,
            $"Creacion del empleado '{empleado.NombreCompleto}' (codigo {empleado.CodigoEmpleado})",
            direccionIp);

        return empleado.Id;
    }

    public async Task ActualizarAsync(EmpleadoDto dto, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var empleado = await _context.Empleados.FindAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Empleado {dto.Id} no encontrado.");
        empleado.CodigoEmpleado = dto.CodigoEmpleado;
        empleado.NombreCompleto = dto.NombreCompleto;
        empleado.Cedula = dto.Cedula;
        empleado.DepartamentoId = dto.DepartamentoId;
        empleado.Cargo = dto.Cargo;
        empleado.Correo = dto.Correo;
        empleado.TelefonoMovil = dto.TelefonoMovil;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Modificacion,
            "Empleado", empleado.Id,
            $"Actualizacion de datos del empleado '{empleado.NombreCompleto}' (codigo {empleado.CodigoEmpleado})",
            direccionIp);
    }

    public async Task CambiarEstadoAsync(int id, bool activo, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var empleado = await _context.Empleados.FindAsync(id)
            ?? throw new KeyNotFoundException($"Empleado {id} no encontrado.");
        empleado.Activo = activo;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Modificacion,
            "Empleado", empleado.Id,
            $"Empleado '{empleado.NombreCompleto}' {(activo ? "activado" : "desactivado")}",
            direccionIp);
    }
}