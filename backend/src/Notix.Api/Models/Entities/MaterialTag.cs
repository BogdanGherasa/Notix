namespace Notix.Api.Models.Entities;

/// <summary>Join table Material ↔ Tag, with composite primary key (MaterialId, TagId).</summary>
public class MaterialTag
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
