using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Notix.Api.Data;

namespace Notix.Api.Repositories;

public class Repository<T>(NotixDbContext db) : IRepository<T> where T : class
{
    protected NotixDbContext Db { get; } = db;
    protected DbSet<T> Set => Db.Set<T>();

    public async Task<T?> GetByIdAsync(params object[] keyValues) => await Set.FindAsync(keyValues);

    public Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate is null ? Set.ToListAsync() : Set.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await Set.AddAsync(entity);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task<int> SaveChangesAsync() => Db.SaveChangesAsync();
}
