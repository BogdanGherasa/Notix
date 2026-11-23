namespace Notix.Api.Models.Entities;

public class Subject : BaseEntity
{
    public int FacultyId { get; set; }
    public Faculty Faculty { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public int StudyYear { get; set; }

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
