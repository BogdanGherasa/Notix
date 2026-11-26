namespace Notix.Api.Repositories;

public enum MaterialSort
{
    Newest,
    PriceAsc,
    PriceDesc,
    Title
}

// Prices are in bani; the service converts the lei values received from the API.
public record MaterialCatalogQuery
{
    public const int DefaultPageSize = 12;

    public string? Search { get; init; }
    public int? UniversityId { get; init; }
    public int? FacultyId { get; init; }
    public int? CategoryId { get; init; }
    public int? MinPriceBani { get; init; }
    public int? MaxPriceBani { get; init; }
    public MaterialSort Sort { get; init; } = MaterialSort.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;
}
