namespace Notix.Api.Models.Enums;

/// <summary>Role of a user account. Stored as int, guarded by a CHECK constraint.</summary>
public enum UserRole
{
    Buyer = 1,
    Seller = 2,
    Admin = 3
}
