using GestionTicketsCombustible.Application.Vehiculos;
using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class VehiculoService : IVehiculoService
{
    private readonly ApplicationDbContext _context;

    public VehiculoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehiculoDto>> ObtenerTodosAsync()
    {
        return await _context.Vehiculos
            .Include(v => v.Departamento)
            .Select(v => new VehiculoDto
            {
                Id = v.Id,
                Placa = v.Placa,
                Ficha = v.Ficha,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                Tipo = v.Tipo,
                DepartamentoId = v.DepartamentoId,
                DepartamentoNombre = v.Departamento.Nombre,
                CapacidadTanque = v.CapacidadTanque,
                Kilometraje = v.Kilometraje,
                Activo = v.Activo
            })
            .ToListAsync();
    }

    public async Task<VehiculoDto?> ObtenerPorIdAsync(int id)
    {
        var v = await _context.Vehiculos.Include(v => v.Departamento).FirstOrDefaultAsync(v => v.Id == id);
        if (v is null) return null;
        return new VehiculoDto
        {
            Id = v.Id,
            Placa = v.Placa,
            Ficha = v.Ficha,
            Marca = v.Marca,
            Modelo = v.Modelo,
            Anio = v.Anio,
            Tipo = v.Tipo,
            DepartamentoId = v.DepartamentoId,
            DepartamentoNombre = v.Departamento.Nombre,
            CapacidadTanque = v.CapacidadTanque,
            Kilometraje = v.Kilometraje,
            Activo = v.Activo
        };
    }

    public async Task<int> CrearAsync(VehiculoDto dto)
    {
        var vehiculo = new Vehiculo
        {
            Placa = dto.Placa,
            Ficha = dto.Ficha,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Anio = dto.Anio,
            Tipo = dto.Tipo,
            DepartamentoId = dto.DepartamentoId,
            CapacidadTanque = dto.CapacidadTanque,
            Kilometraje = dto.Kilometraje,
            Activo = true
        };
        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync();
        return vehiculo.Id;
    }

    public async Task ActualizarAsync(VehiculoDto dto)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Vehiculo {dto.Id} no encontrado.");
        vehiculo.Placa = dto.Placa;
        vehiculo.Ficha = dto.Ficha;
        vehiculo.Marca = dto.Marca;
        vehiculo.Modelo = dto.Modelo;
        vehiculo.Anio = dto.Anio;
        vehiculo.Tipo = dto.Tipo;
        vehiculo.DepartamentoId = dto.DepartamentoId;
        vehiculo.CapacidadTanque = dto.CapacidadTanque;
        vehiculo.Kilometraje = dto.Kilometraje;
        await _context.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id)
            ?? throw new KeyNotFoundException($"Vehiculo {id} no encontrado.");
        vehiculo.Activo = activo;
        await _context.SaveChangesAsync();
    }
}