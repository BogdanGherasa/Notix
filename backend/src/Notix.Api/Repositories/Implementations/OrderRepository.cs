using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;
using Notix.Api.Models.Enums;
using Notix.Api.Repositories.Interfaces;

namespace Notix.Api.Repositories.Implementations;

public class OrderRepository(NotixDbContext db) : Repository<Order>(db), IOrderRepository
{
    public Task<Order?> GetByStripeSessionIdAsync(string stripeSessionId) => Set
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.StripeSessionId == stripeSessionId);

    public Task<bool> HasPurchasedAsync(int userId, int materialId) => Db.OrderItems
        .AnyAsync(i => i.MaterialId == materialId
                       && i.Order.BuyerId == userId
                       && i.Order.Status == OrderStatus.Paid);
}
