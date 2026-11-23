using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects", t =>
            t.HasCheckConstraint("CK_Subjects_StudyYear", "\"StudyYear\" BETWEEN 1 AND 6"));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);

        builder.HasIndex(s => new { s.FacultyId, s.Name, s.StudyYear }).IsUnique();

        builder.HasOne(s => s.Faculty)
            .WithMany(f => f.Subjects)
            .HasForeignKey(s => s.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
