namespace Notix.Api.Models.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
