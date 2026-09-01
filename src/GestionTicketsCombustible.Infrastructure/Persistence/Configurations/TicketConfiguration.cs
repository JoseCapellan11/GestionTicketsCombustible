using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.NumeroTicket).HasMaxLength(30).IsRequired();
        builder.HasIndex(t => t.NumeroTicket).IsUnique();

        builder.HasIndex(t => t.TicketUid).IsUnique();

        builder.Property(t => t.HashSeguridad).HasMaxLength(64);

        builder.Property(t => t.TokenValidacion).HasMaxLength(64);
        builder.HasIndex(t => t.TokenValidacion).IsUnique();

        builder.Property(t => t.EmpleadoNombreSnapshot).HasMaxLength(150);
        builder.Property(t => t.VehiculoPlacaSnapshot).HasMaxLength(20);
        builder.Property(t => t.DepartamentoNombreSnapshot).HasMaxLength(100);

        builder.Property(t => t.CantidadAutorizada).HasPrecision(10, 2);
        builder.Property(t => t.TipoCombustible).HasMaxLength(50);

        builder.Property(t => t.Estado).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(t => t.Solicitud)
            .WithOne(s => s.Ticket)
            .HasForeignKey<Ticket>(t => t.SolicitudId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => t.SolicitudId).IsUnique();

        builder.HasOne(t => t.Empleado)
            .WithMany()
            .HasForeignKey(t => t.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Vehiculo)
            .WithMany()
            .HasForeignKey(t => t.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Departamento)
            .WithMany()
            .HasForeignKey(t => t.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}