namespace Notix.Api.Models.Entities;

public class Subject : BaseEntity
{
    public int FacultyId { get; set; }
    public Faculty Faculty { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    /// <summary>Year of study (1–6). Belongs to the subject, not to the material (3NF).</summary>
    public int StudyYear { get; set; }

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
