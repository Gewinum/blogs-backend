using Blogs.Domain.Models;

namespace Blogs.Domain.Interfaces.Repositories;

public interface IBlogRepository : IRepository<Blog>
{
    public Task<IEnumerable<Blog>> GetPageAsync(int page, int pageSize);
}