using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(254);
        builder.Property(c => c.Subject).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(3000);

        // Optional FK: the sender may be anonymous. No navigation collection on User.
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
