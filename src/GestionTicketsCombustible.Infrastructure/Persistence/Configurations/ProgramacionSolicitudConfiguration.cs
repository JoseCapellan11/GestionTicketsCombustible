using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class ProgramacionSolicitudConfiguration : IEntityTypeConfiguration<ProgramacionSolicitud>
{
    public void Configure(EntityTypeBuilder<ProgramacionSolicitud> builder)
    {
        builder.Property(p => p.CantidadAutorizada).HasPrecision(10, 2);
        builder.Property(p => p.TipoCombustible).HasMaxLength(50);
        builder.Property(p => p.Frecuencia).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(p => p.Empleado)
            .WithMany()
            .HasForeignKey(p => p.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Vehiculo)
            .WithMany()
            .HasForeignKey(p => p.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Departamento)
            .WithMany()
            .HasForeignKey(p => p.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}