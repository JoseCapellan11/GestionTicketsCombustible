using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class DespachoConfiguration : IEntityTypeConfiguration<Despacho>
{
    public void Configure(EntityTypeBuilder<Despacho> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.GalonesDespachados)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(d => d.Estacion)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Observaciones)
            .HasMaxLength(500);

        builder.Property(d => d.FechaHoraDespacho)
            .IsRequired();

        builder.HasIndex(d => d.TicketId)
            .IsUnique();

        builder.HasOne(d => d.Ticket)
            .WithOne(t => t.Despacho)
            .HasForeignKey<Despacho>(d => d.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.UsuarioDespachadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Tanque)
            .WithMany()
            .HasForeignKey(d => d.TanqueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}