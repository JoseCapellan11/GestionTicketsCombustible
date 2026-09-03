using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class CierreDiarioConfiguration : IEntityTypeConfiguration<CierreDiario>
{
    public void Configure(EntityTypeBuilder<CierreDiario> builder)
    {
        builder.Property(c => c.UsuarioCierreNombreSnapshot).HasMaxLength(256);
        builder.Property(c => c.Observaciones).HasMaxLength(1000);

        // RF-18: no se puede generar mas de un cierre para el mismo dia
        builder.HasIndex(c => c.Fecha).IsUnique();

        builder.HasMany(c => c.Detalles)
            .WithOne(d => d.CierreDiario)
            .HasForeignKey(d => d.CierreDiarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}