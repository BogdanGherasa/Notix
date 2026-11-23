using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class MaterialPreviewConfiguration : IEntityTypeConfiguration<MaterialPreview>
{
    public void Configure(EntityTypeBuilder<MaterialPreview> builder)
    {
        builder.ToTable("MaterialPreviews", t =>
            t.HasCheckConstraint("CK_MaterialPreviews_SortOrder", "\"SortOrder\" BETWEEN 1 AND 4"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FileName).IsRequired().HasMaxLength(100);

        builder.HasIndex(p => new { p.MaterialId, p.SortOrder }).IsUnique();

        builder.HasOne(p => p.Material)
            .WithMany(m => m.Previews)
            .HasForeignKey(p => p.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
