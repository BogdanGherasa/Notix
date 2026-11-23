using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags", t =>
            t.HasCheckConstraint("CK_Tags_Name_Lowercase", "\"Name\" = lower(\"Name\")"));

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(30);

        builder.HasIndex(t => t.Name).IsUnique();
    }
}
