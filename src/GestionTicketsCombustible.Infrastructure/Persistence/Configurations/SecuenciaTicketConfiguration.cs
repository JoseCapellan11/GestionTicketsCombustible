using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class SecuenciaTicketConfiguration : IEntityTypeConfiguration<SecuenciaTicket>
{
    public void Configure(EntityTypeBuilder<SecuenciaTicket> builder)
    {
        builder.HasIndex(s => s.Anio).IsUnique();
    }
}