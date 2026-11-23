namespace Notix.Api.Models.Enums;

/// <summary>Moderation status of a material. Stored as int, guarded by a CHECK constraint.</summary>
public enum MaterialStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
