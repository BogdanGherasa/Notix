using Notix.Api.Models.Enums;

namespace Notix.Api.Models.Entities;

public class User : BaseEntity
{
    /// <summary>Always stored lowercase.</summary>
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsBlocked { get; set; }

    /// <summary>GUID string; changed on block or password reset to invalidate existing cookies.</summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}
