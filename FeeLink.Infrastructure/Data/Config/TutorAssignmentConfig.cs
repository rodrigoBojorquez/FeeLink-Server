using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class TutorAssignmentConfig : IEntityTypeConfiguration<TutorAssignment>
{
    public void Configure(EntityTypeBuilder<TutorAssignment> b)
    {
        b.HasKey(x => x.Id);
        b.HasOne(x => x.User)
            .WithMany(u => u.TutorAssignments)
            .HasForeignKey(x => x.UserId);
        b.HasOne(x => x.Patient)
            .WithMany(p => p.TutorAssignments)
            .HasForeignKey(x => x.PatientId);
    }
}