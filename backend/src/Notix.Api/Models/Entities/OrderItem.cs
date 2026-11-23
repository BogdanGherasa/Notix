namespace Notix.Api.Models.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    /// <summary>Price at the moment of purchase, in bani. Not redundant: the material price may change later.</summary>
    public int UnitPriceBani { get; set; }
}
