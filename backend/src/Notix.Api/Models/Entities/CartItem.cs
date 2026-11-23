namespace Notix.Api.Models.Entities;

public class CartItem
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public DateTime AddedAt { get; set; }
}
