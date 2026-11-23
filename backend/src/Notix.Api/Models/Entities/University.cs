namespace Notix.Api.Models.Entities;

public class University : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
}
