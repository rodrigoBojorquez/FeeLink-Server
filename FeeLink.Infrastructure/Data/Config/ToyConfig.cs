using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class ToyConfig : IEntityTypeConfiguration<Toy>
{
    public void Configure(EntityTypeBuilder<Toy> builder)
    {
        builder.HasMany<SensorReading>().WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}