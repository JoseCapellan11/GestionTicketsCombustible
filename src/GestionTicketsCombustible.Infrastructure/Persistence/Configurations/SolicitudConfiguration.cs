using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class SolicitudConfiguration : IEntityTypeConfiguration<Solicitud>
{
    public void Configure(EntityTypeBuilder<Solicitud> builder)
    {
        builder.Property(s => s.CantidadAutorizada).HasPrecision(10, 2);
        builder.Property(s => s.TipoCombustible).HasMaxLength(50);

        builder.Property(s => s.TipoSolicitud).HasConversion<string>().HasMaxLength(30);
        builder.Property(s => s.Estado).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(s => s.Empleado)
            .WithMany()
            .HasForeignKey(s => s.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Vehiculo)
            .WithMany()
            .HasForeignKey(s => s.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Departamento)
            .WithMany()
            .HasForeignKey(s => s.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sin navegacion en Domain: la FK real hacia AspNetUsers se declara solo aqui.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(s => s.UsuarioSolicitanteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false); // Permitir null para solicitudes generadas por ProgramacionSolicitud
    }
}