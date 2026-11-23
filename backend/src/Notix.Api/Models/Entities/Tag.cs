namespace Notix.Api.Models.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<MaterialTag> MaterialTags { get; set; } = new List<MaterialTag>();
}
