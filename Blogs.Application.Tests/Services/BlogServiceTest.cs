using AutoMapper;
using Blogs.Application.Exceptions;
using Blogs.Application.Mapping;
using Blogs.Application.Services;
using Blogs.Domain.Interfaces.Database;
using Blogs.Domain.Interfaces.Repositories;
using Blogs.Domain.Models;
using Moq;

namespace Blogs.Application.Tests.Services;

public class BlogServiceTest
{
    [Fact]
    public async Task GetBlogsAsync_ShouldReturnBlogs()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        var blogs = CreateBlogs(10);
        blogRepository.Setup(x => x.GetPageAsync(1, 10)).ReturnsAsync(blogs);
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        // Act
        var result = await blogService.GetBlogsAsync(1, 10);
        
        // Assert
        var resultArray = result.ToArray();
        Assert.NotNull(result);
        Assert.Equal(blogs.Length, resultArray.Length);
        for (var i = 0; i < blogs.Length; i++)
        {
            Assert.Equal(blogs[i].Id, resultArray[i].Id);
            Assert.Equal(blogs[i].Title, resultArray[i].Title);
            Assert.Equal(blogs[i].Content, resultArray[i].Content);
            Assert.Equal(blogs[i].CreatedAt, resultArray[i].CreatedAt);
        }
        blogRepository.Verify(b => b.GetPageAsync(1, 10), Times.Once);
    }
    
    [Fact]
    public async Task GetBlogsAsync_ShouldReturnEmptyBlogs()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        blogRepository.Setup(x => x.GetPageAsync(1, 10)).ReturnsAsync(Array.Empty<Blog>());
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        // Act
        var result = await blogService.GetBlogsAsync(1, 10);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        blogRepository.Verify(b => b.GetPageAsync(1, 10), Times.Once);
    }
    
    [Fact]
    public async Task GetBlogAsync_ShouldReturnBlog()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);

        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.GetByIdAsync(blog.Id)).ReturnsAsync(blog);
        
        // Act
        var result = await blogService.GetBlogAsync(blog.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(blog.Id, result.Id);
        Assert.Equal(blog.Title, result.Title);
        Assert.Equal(blog.Content, result.Content);
        Assert.Equal(blog.CreatedAt, result.CreatedAt);
        blogRepository.Verify(b => b.GetByIdAsync(blog.Id), Times.Once);
    }
    
    [Fact]
    public async Task GetBlogAsync_NotFound()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var id = Guid.NewGuid();
        
        blogRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(null as Blog);
        
        // Act
        await Assert.ThrowsAsync<BlogNotFoundException>(() => blogService.GetBlogAsync(id));
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task CreateBlogAsync_ShouldCreateBlog()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.AddAsync(It.IsAny<Blog>())).ReturnsAsync(blog);
        
        // Act
        var result = await blogService.CreateBlogAsync(blog.Title, blog.Description, blog.Content, blog.Author);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(blog.Id, result.Id);
        Assert.Equal(blog.Title, result.Title);
        Assert.Equal(blog.Content, result.Content);
        Assert.Equal(blog.Author, result.Author);
        Assert.Equal(blog.CreatedAt, result.CreatedAt);
        Assert.Equal(blog.UpdatedAt, result.UpdatedAt);
        blogRepository.Verify(b => b.AddAsync(It.IsAny<Blog>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateBlogTitleAsync_ShouldUpdateTitle()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.GetByIdAsync(blog.Id)).ReturnsAsync(blog);
        
        // Act
        await blogService.UpdateBlogTitleAsync(blog.Id, "New Title");
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(blog.Id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBlogTitleAsync_NotExists()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var id = Guid.NewGuid();
        
        blogRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(null as Blog);
        
        // Act
        await Assert.ThrowsAsync<BlogNotFoundException>(() => blogService.UpdateBlogTitleAsync(id, "New Title"));
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Never);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateBlogDescriptionAsync_ShouldUpdateDescription()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.GetByIdAsync(blog.Id)).ReturnsAsync(blog);
        
        // Act
        await blogService.UpdateBlogDescriptionAsync(blog.Id, "New Description");
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(blog.Id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBlogDescriptionAsync_NotExists()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var id = Guid.NewGuid();
        
        blogRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(null as Blog);
        
        // Act
        await Assert.ThrowsAsync<BlogNotFoundException>(() => blogService.UpdateBlogDescriptionAsync(id, "New Description"));
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Never);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateBlogContentAsync_ShouldUpdateContent()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.GetByIdAsync(blog.Id)).ReturnsAsync(blog);
        
        // Act
        await blogService.UpdateBlogContentAsync(blog.Id, "New Content");
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(blog.Id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateBlogContentAsync_NotExists()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var id = Guid.NewGuid();
        
        blogRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(null as Blog);
        
        // Act
        await Assert.ThrowsAsync<BlogNotFoundException>(() => blogService.UpdateBlogContentAsync(id, "New Content"));
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(id), Times.Once);
        blogRepository.Verify(b => b.UpdateAsync(It.IsAny<Blog>()), Times.Never);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteBlogAsync_ShouldDeleteBlog()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var blog = CreateBlog();
        
        blogRepository.Setup(x => x.GetByIdAsync(blog.Id)).ReturnsAsync(blog);
        
        // Act
        await blogService.DeleteBlogAsync(blog.Id);
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(blog.Id), Times.Once);
        blogRepository.Verify(b => b.DeleteAsync(It.IsAny<Blog>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteBlogAsync_NotExists()
    {
        // Arrange
        var blogRepository = new Mock<IBlogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        
        var blogService = new BlogService(blogRepository.Object, unitOfWork.Object, mapper);
        
        var id = Guid.NewGuid();
        
        blogRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(null as Blog);
        
        // Act
        await Assert.ThrowsAsync<BlogNotFoundException>(() => blogService.DeleteBlogAsync(id));
        
        // Assert
        blogRepository.Verify(b => b.GetByIdAsync(id), Times.Once);
        blogRepository.Verify(b => b.DeleteAsync(It.IsAny<Blog>()), Times.Never);
        unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        unitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    private static Blog[] CreateBlogs(int amount)
    {
        var blogs = new Blog[amount];
        for (var i = 0; i < amount; i++)
        {
            blogs[i] = CreateBlog();
        }
        return blogs;
    }
    
    private static Blog CreateBlog()
    {
        return new Blog
        {
            Id = Guid.NewGuid(),
            Title = RandomString(10),
            Description = RandomString(200),
            Content = RandomString(3000),
            Author = RandomString(20),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
    
    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}