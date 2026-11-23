using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class MaterialTagConfiguration : IEntityTypeConfiguration<MaterialTag>
{
    public void Configure(EntityTypeBuilder<MaterialTag> builder)
    {
        builder.ToTable("MaterialTags");

        builder.HasKey(mt => new { mt.MaterialId, mt.TagId });

        builder.HasOne(mt => mt.Material)
            .WithMany(m => m.MaterialTags)
            .HasForeignKey(mt => mt.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mt => mt.Tag)
            .WithMany(t => t.MaterialTags)
            .HasForeignKey(mt => mt.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
