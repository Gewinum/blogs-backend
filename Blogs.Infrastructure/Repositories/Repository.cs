using Blogs.Domain.Interfaces.Models;
using Blogs.Domain.Interfaces.Repositories;
using Blogs.Infrastructure.Database;
using Blogs.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Infrastructure.Repositories;

public abstract class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class
{ 
    protected readonly DbSet<T> dbSet = context.Set<T>();
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await dbSet.FindAsync(id);
    }
    
    public async Task<T?> GetWithLockAsync(Guid id)
    {
        var result = dbSet.FromSqlRaw("SELECT * FROM " + dbSet.EntityType.GetTableName() + " WITH (UPDLOCK) WHERE Id = {0}", id).AsAsyncEnumerable();
        await using var asyncEnumerator = result.GetAsyncEnumerator();
        var hasResult = await asyncEnumerator.MoveNextAsync();
        return hasResult ? asyncEnumerator.Current : null;
    }

    public async Task<T?> AddAsync(T entity)
    {
        if (entity is IModel model)
        {
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
        }

        await dbSet.AddAsync(entity);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is not SqlException innerException)
            {
                throw e.InnerException ?? e;
            }
            
            if (innerException.Number == 2601)
            {
                throw new DuplicateKeysException();
            }

            throw e.InnerException ?? e;
        }
        return entity;
    }
    
    public async Task UpdateAsync(T entity)
    {
        if (entity is IModel model)
        {
            model.UpdatedAt = DateTime.UtcNow;
        }
        
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        dbSet.Remove(entity);
        await context.SaveChangesAsync();
    }
}