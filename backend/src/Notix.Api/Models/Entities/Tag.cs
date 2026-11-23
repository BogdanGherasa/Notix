namespace Notix.Api.Models.Entities;

public class Tag : BaseEntity
{
    /// <summary>Always stored lowercase.</summary>
    public string Name { get; set; } = string.Empty;

    public ICollection<MaterialTag> MaterialTags { get; set; } = new List<MaterialTag>();
}
