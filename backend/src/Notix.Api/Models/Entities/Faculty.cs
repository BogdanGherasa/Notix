namespace Notix.Api.Models.Entities;

public class Faculty : BaseEntity
{
    public int UniversityId { get; set; }
    public University University { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
