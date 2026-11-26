using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public class ReviewRepository(NotixDbContext db) : Repository<Review>(db), IReviewRepository
{
    public Task<List<Review>> GetForMaterialAsync(int materialId) => Set
        .Where(r => r.MaterialId == materialId)
        .Include(r => r.Buyer)
        .OrderByDescending(r => r.CreatedAt)
        .ThenByDescending(r => r.Id)
        .AsNoTracking()
        .ToListAsync();

    public Task<bool> ExistsAsync(int materialId, int buyerId) =>
        Set.AnyAsync(r => r.MaterialId == materialId && r.BuyerId == buyerId);

    public async Task<RatingSummary> GetRatingSummaryAsync(int materialId)
    {
        var reviews = Set.Where(r => r.MaterialId == materialId);
        var count = await reviews.CountAsync();
        var average = await reviews.AverageAsync(r => (double?)r.Rating);
        return new RatingSummary(average, count);
    }
}
