using Notix.Api.Models.Entities;
using Notix.Api.Repositories.Common;

namespace Notix.Api.Repositories.Interfaces;

public interface IMaterialRepository : IRepository<Material>
{
    Task<PagedResult<Material>> GetCatalogAsync(MaterialCatalogQuery query);
    Task<Material?> GetDetailsAsync(int id);
    Task<List<Material>> GetSimilarAsync(Material material, int count = 4);
    Task<List<Material>> GetFeaturedAsync(int count = 6);
}
