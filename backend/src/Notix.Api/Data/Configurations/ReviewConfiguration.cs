using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", t =>
            t.HasCheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5"));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment).HasMaxLength(1000);

        builder.HasIndex(r => new { r.MaterialId, r.BuyerId }).IsUnique();

        builder.HasOne(r => r.Material)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Buyer)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
