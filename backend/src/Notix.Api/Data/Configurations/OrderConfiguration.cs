using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", t =>
            t.HasCheckConstraint("CK_Orders_Status", "\"Status\" IN (1, 2, 3)"));

        builder.HasKey(o => o.Id);

        builder.Property(o => o.StripeSessionId).HasMaxLength(255);

        // Unique index on a nullable column: SQLite allows many NULLs, so only real session ids must be unique.
        builder.HasIndex(o => o.StripeSessionId).IsUnique();

        builder.HasOne(o => o.Buyer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
