namespace Notix.Api.Models.Entities;

public class MaterialTag
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
