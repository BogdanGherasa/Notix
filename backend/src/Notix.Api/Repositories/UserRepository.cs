using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public class UserRepository(NotixDbContext db) : Repository<User>(db), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        var normalized = Normalize(email);
        return Set.FirstOrDefaultAsync(u => u.Email == normalized);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        var normalized = Normalize(email);
        return Set.AnyAsync(u => u.Email == normalized);
    }

    // Emails are stored lowercase (CHECK constraint), so lookups normalize the same way.
    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
