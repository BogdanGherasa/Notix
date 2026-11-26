using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public interface ICartRepository : IRepository<CartItem>
{
    Task<List<CartItem>> GetItemsAsync(int userId);
    Task<CartItem?> GetItemAsync(int userId, int materialId);
    Task<bool> ContainsAsync(int userId, int materialId);
    Task RemoveItemsAsync(int userId, IEnumerable<int> materialIds);
}
