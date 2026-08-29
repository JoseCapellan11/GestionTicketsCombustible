using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.Property(e => e.CodigoEmpleado).IsRequired().HasMaxLength(20);
        builder.HasIndex(e => e.CodigoEmpleado).IsUnique();

        builder.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
        builder.HasIndex(e => e.Cedula).IsUnique();

        builder.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Correo).IsRequired().HasMaxLength(150);
        builder.Property(e => e.TelefonoMovil).HasMaxLength(20);
        builder.Property(e => e.Cargo).HasMaxLength(100);

        builder.HasOne(e => e.Departamento)
            .WithMany(d => d.Empleados)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}