namespace Notix.Api.Models.Enums;

/// <summary>Payment status of an order. Stored as int, guarded by a CHECK constraint.</summary>
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Canceled = 3
}
