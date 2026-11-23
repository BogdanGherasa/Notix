using Notix.Api.Models.Enums;

namespace Notix.Api.Models.Entities;

/// <summary>
/// A study material for sale. University and faculty are reached through
/// Subject → Faculty → University and are deliberately not stored here (3NF).
/// </summary>
public class Material : BaseEntity
{
    public int SellerId { get; set; }
    public User Seller { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Price in bani (1 leu = 100 bani), between 200 and 50000.</summary>
    public int PriceBani { get; set; }
    public int PageCount { get; set; }
    public long FileSizeBytes { get; set; }

    /// <summary>GUID file name of the PDF inside <c>storage/pdf</c>.</summary>
    public string PdfFileName { get; set; } = string.Empty;

    /// <summary>GUID file name of the cover image inside <c>storage/images</c>.</summary>
    public string CoverFileName { get; set; } = string.Empty;

    public MaterialStatus Status { get; set; } = MaterialStatus.Pending;
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Soft delete. There is intentionally no global query filter: catalog queries exclude
    /// deleted materials explicitly, while library and download still need to see them.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>Last modification moment, UTC. Maintained by <c>NotixDbContext</c>.</summary>
    public DateTime UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public ICollection<MaterialPreview> Previews { get; set; } = new List<MaterialPreview>();
    public ICollection<MaterialTag> MaterialTags { get; set; } = new List<MaterialTag>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
