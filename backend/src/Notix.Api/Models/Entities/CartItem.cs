namespace Notix.Api.Models.Entities;

/// <summary>A material in a user's cart, with composite primary key (UserId, MaterialId).</summary>
public class CartItem
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    /// <summary>Moment the material was added, UTC. Filled automatically by <c>NotixDbContext</c> when left default.</summary>
    public DateTime AddedAt { get; set; }
}
