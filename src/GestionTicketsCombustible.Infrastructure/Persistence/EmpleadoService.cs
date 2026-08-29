using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly ApplicationDbContext _context;

    public EmpleadoService(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<int> CrearAsync(EmpleadoDto dto)
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
        return empleado.Id;
    }

    public async Task ActualizarAsync(EmpleadoDto dto)
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
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        var empleado = await _context.Empleados.FindAsync(id)
            ?? throw new KeyNotFoundException($"Empleado {id} no encontrado.");
        empleado.Activo = activo;
        await _context.SaveChangesAsync();
    }
}