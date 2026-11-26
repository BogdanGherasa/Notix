using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;
using Notix.Api.Models.Entities;
using Notix.Api.Repositories.Interfaces;

namespace Notix.Api.Repositories.Implementations;

public class LookupRepository(NotixDbContext db) : ILookupRepository
{
    public Task<List<Category>> GetCategoriesAsync() => db.Categories
        .OrderBy(c => c.Name)
        .AsNoTracking()
        .ToListAsync();

    public Task<List<University>> GetUniversitiesWithFacultiesAndSubjectsAsync() => db.Universities
        .Include(u => u.Faculties.OrderBy(f => f.Name))
        .ThenInclude(f => f.Subjects.OrderBy(s => s.StudyYear).ThenBy(s => s.Name))
        .OrderBy(u => u.Name)
        .AsSplitQuery()
        .AsNoTracking()
        .ToListAsync();

    public Task<Subject?> GetSubjectAsync(int subjectId) => db.Subjects
        .Include(s => s.Faculty).ThenInclude(f => f.University)
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.Id == subjectId);

    public Task<bool> CategoryExistsAsync(int categoryId) =>
        db.Categories.AnyAsync(c => c.Id == categoryId);
}
