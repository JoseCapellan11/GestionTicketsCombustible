using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.Property(n => n.Mensaje).HasMaxLength(500).IsRequired();
        builder.Property(n => n.EntidadRelacionada).HasMaxLength(100);
        builder.HasIndex(n => n.Leida);
    }
}