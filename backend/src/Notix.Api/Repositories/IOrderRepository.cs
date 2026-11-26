using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByStripeSessionIdAsync(string stripeSessionId);
    Task<bool> HasPurchasedAsync(int userId, int materialId);
}
