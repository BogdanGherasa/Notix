using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;
using Notix.Api.Repositories.Interfaces;

namespace Notix.Api.Repositories.Implementations;

public class CartRepository(NotixDbContext db) : Repository<CartItem>(db), ICartRepository
{
    public Task<List<CartItem>> GetItemsAsync(int userId) => Set
        .Where(c => c.UserId == userId)
        .Include(c => c.Material).ThenInclude(m => m.Category)
        .Include(c => c.Material).ThenInclude(m => m.Subject).ThenInclude(s => s.Faculty).ThenInclude(f => f.University)
        .OrderByDescending(c => c.AddedAt)
        .ToListAsync();

    public Task<CartItem?> GetItemAsync(int userId, int materialId) =>
        Set.FirstOrDefaultAsync(c => c.UserId == userId && c.MaterialId == materialId);

    public Task<bool> ContainsAsync(int userId, int materialId) =>
        Set.AnyAsync(c => c.UserId == userId && c.MaterialId == materialId);

    public async Task RemoveItemsAsync(int userId, IEnumerable<int> materialIds)
    {
        var ids = materialIds.ToList();
        var items = await Set.Where(c => c.UserId == userId && ids.Contains(c.MaterialId)).ToListAsync();
        Set.RemoveRange(items);
    }
}
