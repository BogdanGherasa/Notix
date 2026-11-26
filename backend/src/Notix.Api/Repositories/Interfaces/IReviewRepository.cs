using Notix.Api.Models.Entities;
using Notix.Api.Repositories.Common;

namespace Notix.Api.Repositories.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<List<Review>> GetForMaterialAsync(int materialId);
    Task<bool> ExistsAsync(int materialId, int buyerId);
    Task<RatingSummary> GetRatingSummaryAsync(int materialId);
}
