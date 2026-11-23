namespace Notix.Api.Models.Entities;

public class MaterialPreview : BaseEntity
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    /// <summary>GUID file name of the preview image inside <c>storage/images</c>.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Position in the carousel (1–4).</summary>
    public int SortOrder { get; set; }
}
