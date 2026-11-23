using Notix.Api.Models.Enums;

namespace Notix.Api.Models.Entities;

/// <summary>
/// An order. The total is not stored: it is the sum of <see cref="OrderItem.UnitPriceBani"/> (3NF).
/// </summary>
public class Order : BaseEntity
{
    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? StripeSessionId { get; set; }
    public DateTime? PaidAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
