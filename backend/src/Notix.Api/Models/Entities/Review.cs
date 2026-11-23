namespace Notix.Api.Models.Entities;

public class Review : BaseEntity
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public int Rating { get; set; }
    public string? Comment { get; set; }
}
