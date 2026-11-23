using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notix.Api.Models.Entities;

namespace Notix.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint("CK_Users_Role", "\"Role\" IN (1, 2, 3)");
            t.HasCheckConstraint("CK_Users_Email_Lowercase", "\"Email\" = lower(\"Email\")");
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(254);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.SecurityStamp).IsRequired().HasMaxLength(36);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
