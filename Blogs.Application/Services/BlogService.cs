using AutoMapper;
using Blogs.Application.Exceptions;
using Blogs.Domain.Dtos;
using Blogs.Domain.Interfaces.Database;
using Blogs.Domain.Interfaces.Repositories;
using Blogs.Domain.Interfaces.Services;
using Blogs.Domain.Models;

namespace Blogs.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public BlogService(IBlogRepository blogRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<BlogDto>> GetBlogsAsync(int page, int pageSize)
    {
        var blogs = await _blogRepository.GetPageAsync(page, pageSize);
        return _mapper.Map<IEnumerable<BlogDto>>(blogs);
    }
    
    public async Task<BlogDto> GetBlogAsync(Guid id)
    {
        var blog = await _blogRepository.GetByIdAsync(id);
        if (blog == null)
        {
            throw new BlogNotFoundException(id);
        }
        
        return _mapper.Map<BlogDto>(blog);
    }

    public async Task<BlogDto> CreateBlogAsync(string title, string description, string contents, string author)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var blog = new Blog
            {
                Title = title,
                Description = description,
                Content = contents,
                Author = author
            };
            
            var result = await _blogRepository.AddAsync(blog);
            await _unitOfWork.CommitTransactionAsync();
            return _mapper.Map<BlogDto>(result);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateBlogTitleAsync(Guid id, string title)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                throw new BlogNotFoundException(id);
            }
            
            blog.Title = title;
            await _blogRepository.UpdateAsync(blog);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    
    public async Task UpdateBlogDescriptionAsync(Guid id, string description)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                throw new BlogNotFoundException(id);
            }
            
            blog.Description = description;
            await _blogRepository.UpdateAsync(blog);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    
    public async Task UpdateBlogContentAsync(Guid id, string content)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                throw new BlogNotFoundException(id);
            }
            
            blog.Content = content;
            await _blogRepository.UpdateAsync(blog);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    
    public async Task DeleteBlogAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                throw new BlogNotFoundException(id);
            }
            
            await _blogRepository.DeleteAsync(blog);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}