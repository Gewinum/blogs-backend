using Blogs.Domain.Dtos;

namespace Blogs.Domain.Interfaces.Services;

public interface IBlogService
{
    public Task<IEnumerable<BlogDto>> GetBlogsAsync(int page, int pageSize);
    
    public Task<BlogDto> GetBlogAsync(Guid id);

    public Task<BlogDto> CreateBlogAsync(string title, string description, string contents, string author);
    
    public Task UpdateBlogTitleAsync(Guid id, string title);
    
    public Task UpdateBlogDescriptionAsync(Guid id, string description);
    
    public Task UpdateBlogContentAsync(Guid id, string content);

    public Task DeleteBlogAsync(Guid id);
}