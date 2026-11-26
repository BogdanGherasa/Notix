using Notix.Api.Models.Entities;

namespace Notix.Api.Repositories;

public interface ILookupRepository
{
    Task<List<Category>> GetCategoriesAsync();
    Task<List<University>> GetUniversitiesWithFacultiesAndSubjectsAsync();
    Task<Subject?> GetSubjectAsync(int subjectId);
    Task<bool> CategoryExistsAsync(int categoryId);
}
