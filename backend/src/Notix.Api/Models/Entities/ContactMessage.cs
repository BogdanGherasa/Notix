namespace Notix.Api.Models.Entities;

public class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Set when the sender was logged in.</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }
}
