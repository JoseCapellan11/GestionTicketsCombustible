using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class CierreDiarioDetalleConfiguration : IEntityTypeConfiguration<CierreDiarioDetalle>
{
    public void Configure(EntityTypeBuilder<CierreDiarioDetalle> builder)
    {
        builder.HasOne(d => d.Tanque)
            .WithMany()
            .HasForeignKey(d => d.TanqueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}