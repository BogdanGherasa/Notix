namespace Notix.Api.Models.Entities;

public class MaterialPreview : BaseEntity
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
