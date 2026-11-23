namespace Notix.Api.Models.Entities;

/// <summary>
/// Base class for every entity with a surrogate key. Join tables (MaterialTag, CartItem)
/// do not inherit it, because they use a composite primary key.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    /// <summary>Creation moment, always UTC. Filled automatically by <c>NotixDbContext</c> when left default.</summary>
    public DateTime CreatedAt { get; set; }
}
