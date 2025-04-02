using Blogs.Domain.Interfaces.Repositories;
using Blogs.Domain.Models;
using Blogs.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Infrastructure.Repositories;

public class BlogRepository : Repository<Blog>, IBlogRepository
{
    public BlogRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Blog>> GetPageAsync(int page, int pageSize)
    {
        return await dbSet
            .Select(b => new Blog
            {
                Id = b.Id,
                Author = b.Author,
                Title = b.Title,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                Content = ""
            })
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}