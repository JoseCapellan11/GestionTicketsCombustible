using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class TanqueConfiguration : IEntityTypeConfiguration<Tanque>
{
    public void Configure(EntityTypeBuilder<Tanque> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.TipoCombustible)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Capacidad).HasPrecision(12, 2);
        builder.Property(t => t.ExistenciaActual).HasPrecision(12, 2);
        builder.Property(t => t.NivelCritico).HasPrecision(12, 2);
    }
}