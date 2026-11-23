using Notix.Api.Models.Enums;

namespace Notix.Api.Models.Entities;

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

    public int PriceBani { get; set; }
    public int PageCount { get; set; }
    public long FileSizeBytes { get; set; }

    public string PdfFileName { get; set; } = string.Empty;
    public string CoverFileName { get; set; } = string.Empty;

    public MaterialStatus Status { get; set; } = MaterialStatus.Pending;
    public string? RejectionReason { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public ICollection<MaterialPreview> Previews { get; set; } = new List<MaterialPreview>();
    public ICollection<MaterialTag> MaterialTags { get; set; } = new List<MaterialTag>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
