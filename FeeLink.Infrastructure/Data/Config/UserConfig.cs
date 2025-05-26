using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany<TherapistAssignment>().WithOne()
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany<TutorAssignment>().WithOne()
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}