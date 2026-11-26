using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;
using Notix.Api.Models.Enums;
using Notix.Api.Repositories.Common;
using Notix.Api.Repositories.Interfaces;

namespace Notix.Api.Repositories.Implementations;

public class MaterialRepository(NotixDbContext db) : Repository<Material>(db), IMaterialRepository
{
    public async Task<PagedResult<Material>> GetCatalogAsync(MaterialCatalogQuery query)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var materials = Published();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = LikePattern.Contains(query.Search.Trim());
            materials = materials.Where(m => EF.Functions.Like(m.Title, pattern, LikePattern.EscapeCharacter));
        }

        if (query.UniversityId is int universityId)
            materials = materials.Where(m => m.Subject.Faculty.UniversityId == universityId);
        if (query.FacultyId is int facultyId)
            materials = materials.Where(m => m.Subject.FacultyId == facultyId);
        if (query.CategoryId is int categoryId)
            materials = materials.Where(m => m.CategoryId == categoryId);
        if (query.MinPriceBani is int minPrice)
            materials = materials.Where(m => m.PriceBani >= minPrice);
        if (query.MaxPriceBani is int maxPrice)
            materials = materials.Where(m => m.PriceBani <= maxPrice);

        var total = await materials.CountAsync();

        var items = await WithCatalogData(Sorted(materials, query.Sort)
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .ToListAsync();

        return new PagedResult<Material>(items, total, page, pageSize);
    }

    public Task<Material?> GetDetailsAsync(int id) => Set
        .Include(m => m.Seller)
        .Include(m => m.Category)
        .Include(m => m.Subject).ThenInclude(s => s.Faculty).ThenInclude(f => f.University)
        .Include(m => m.MaterialTags).ThenInclude(mt => mt.Tag)
        .Include(m => m.Previews.OrderBy(p => p.SortOrder))
        .AsSplitQuery()
        .FirstOrDefaultAsync(m => m.Id == id);

    public Task<List<Material>> GetSimilarAsync(Material material, int count = 4) => WithCatalogData(Published()
            .Where(m => m.Id != material.Id
                        && (m.SubjectId == material.SubjectId || m.CategoryId == material.CategoryId))
            .OrderByDescending(m => m.SubjectId == material.SubjectId)
            .ThenByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(count))
        .ToListAsync();

    public Task<List<Material>> GetFeaturedAsync(int count = 6) => WithCatalogData(Published()
            .OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0)
            .ThenByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(count))
        .ToListAsync();

    private IQueryable<Material> Published() =>
        Set.Where(m => m.Status == MaterialStatus.Approved && !m.IsDeleted);

    private static IQueryable<Material> WithCatalogData(IQueryable<Material> materials) => materials
        .Include(m => m.Category)
        .Include(m => m.Subject).ThenInclude(s => s.Faculty).ThenInclude(f => f.University)
        .AsNoTracking();

    private static IQueryable<Material> Sorted(IQueryable<Material> materials, MaterialSort sort) => sort switch
    {
        MaterialSort.PriceAsc => materials.OrderBy(m => m.PriceBani).ThenBy(m => m.Id),
        MaterialSort.PriceDesc => materials.OrderByDescending(m => m.PriceBani).ThenBy(m => m.Id),
        MaterialSort.Title => materials.OrderBy(m => m.Title).ThenBy(m => m.Id),
        _ => materials.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id)
    };
}
