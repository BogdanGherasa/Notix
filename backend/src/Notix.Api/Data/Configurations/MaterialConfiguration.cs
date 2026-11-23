using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials", t =>
        {
            t.HasCheckConstraint("CK_Materials_PriceBani", "\"PriceBani\" BETWEEN 200 AND 50000");
            t.HasCheckConstraint("CK_Materials_Status", "\"Status\" IN (1, 2, 3)");
        });

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title).IsRequired().HasMaxLength(120);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(3000);
        builder.Property(m => m.PdfFileName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.CoverFileName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.RejectionReason).HasMaxLength(500);

        // No HasQueryFilter(m => !m.IsDeleted) on purpose: see Material.IsDeleted.

        builder.HasOne(m => m.Seller)
            .WithMany(u => u.Materials)
            .HasForeignKey(m => m.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Subject)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Category)
            .WithMany(c => c.Materials)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
