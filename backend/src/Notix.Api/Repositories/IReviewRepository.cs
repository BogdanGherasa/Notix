using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public record RatingSummary(double? Average, int Count);

public interface IReviewRepository : IRepository<Review>
{
    Task<List<Review>> GetForMaterialAsync(int materialId);
    Task<bool> ExistsAsync(int materialId, int buyerId);
    Task<RatingSummary> GetRatingSummaryAsync(int materialId);
}
