using System.Net.Http.Json;
using System.Text.Json;
using Blogs.API.Requests.Blogs;
using Blogs.Domain.Dtos;
using Blogs.Infrastructure.Database;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace Blogs.API.Tests;

public class SqlServerFixture : IAsyncLifetime
{
    public MsSqlContainer MsSqlContainer { get; }
    public string? ConnectionString { get; private set; }

    public SqlServerFixture()
    {
        MsSqlContainer = new MsSqlBuilder()
            .WithPassword("Password123")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
        var masterConnectionString = MsSqlContainer.GetConnectionString();

        // Create the blogs database
        await using (var connection = new SqlConnection(masterConnectionString))
        {
            await connection.OpenAsync();
            await using var command = new SqlCommand("CREATE DATABASE blogs;", connection);
            await command.ExecuteNonQueryAsync();
        }

        ConnectionString = $"{masterConnectionString};Database=blogs";
    }

    public async Task DisposeAsync()
    {
        await MsSqlContainer.DisposeAsync();
    }
}

public class BlogTests : IClassFixture<SqlServerFixture>, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly SqlServerFixture _sqlFixture;

    public BlogTests(WebApplicationFactory<Program> factory, SqlServerFixture sqlFixture)
    {
        _sqlFixture = sqlFixture;

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var existingService =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (existingService != null)
                {
                    services.Remove(existingService);
                }
                
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(_sqlFixture.ConnectionString);
                });
                
                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task GetAndCreateBlog_Success()
    {
        var title = "Test Title";
        var description = "Test Description";
        var content = "Test Content";
        var author = "Test Author";
        
        var dto = await CreateBlog(title, description, content, author);
        Assert.Equal(title, dto.Title);
        Assert.Equal(description, dto.Description);
        Assert.Equal(content, dto.Content);
        Assert.Equal(author, dto.Author);
        Assert.InRange(dto.CreatedAt, DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)), DateTime.UtcNow.Add(TimeSpan.FromSeconds(1)));
        Assert.InRange(dto.UpdatedAt, DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)), DateTime.UtcNow.Add(TimeSpan.FromSeconds(1)));
        
        var id = dto.Id;
        var dto2 = await GetBlogNotNull(id);
        Assert.Equal(dto.Title, dto2.Title);
        Assert.Equal(dto.Description, dto2.Description);
        Assert.Equal(dto.Content, dto2.Content);
        Assert.Equal(dto.Author, dto2.Author);
        Assert.Equal(dto.CreatedAt, dto2.CreatedAt);
        Assert.Equal(dto.UpdatedAt, dto2.UpdatedAt);
    }
    
    [Fact]
    public async Task GetBlog_NotExists()
    {
        var id = Guid.NewGuid();
        
        var blogDto = await GetBlog(id);
        Assert.Null(blogDto);
    }

    [Fact]
    public async Task GetBlog_NoGuid()
    {
        var response = await _client.GetAsync("/blog/invalid");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBlogTitle_Success()
    {
        var title = "Test Title";
        var description = "Test Description";
        var content = "Test Content";
        var author = "Test Author";
        
        var dto = await CreateBlog(title, description, content, author);
        var id = dto.Id;
        
        var newTitle = "New Title";
        var response = await _client.PutAsJsonAsync($"/blog/title/{id}", newTitle);
        response.EnsureSuccessStatusCode();
        
        var dto2 = await GetBlogNotNull(id);
        Assert.Equal(newTitle, dto2.Title);
    }
    
    [Fact]
    public async Task UpdateBlogTitle_NotExists()
    {
        var id = Guid.NewGuid();
        var newTitle = "New Title";
        
        var response = await _client.PutAsJsonAsync($"/blog/title/{id}", newTitle);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBlogDescription_Success()
    {
        var title = "Test Title";
        var description = "Test Description";
        var content = "Test Content";
        var author = "Test Author";
        
        var dto = await CreateBlog(title, description, content, author);
        var id = dto.Id;
        
        var newDescription = "New Description";
        var response = await _client.PutAsJsonAsync($"/blog/description/{id}", newDescription);
        response.EnsureSuccessStatusCode();
        
        var dto2 = await GetBlogNotNull(id);
        Assert.Equal(newDescription, dto2.Description);
    }
    
    [Fact]
    public async Task UpdateBlogDescription_NotExists()
    {
        var id = Guid.NewGuid();
        var newDescription = "New Description";
        
        var response = await _client.PutAsJsonAsync($"/blog/description/{id}", newDescription);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBlogContent_Success()
    {
        var title = "Test Title";
        var description = "Test Description";
        var content = "Test Content";
        var author = "Test Author";
        
        var dto = await CreateBlog(title, description, content, author);
        var id = dto.Id;
        
        var newContent = "New Content";
        var response = await _client.PutAsJsonAsync($"/blog/content/{id}", newContent);
        response.EnsureSuccessStatusCode();
        
        var dto2 = await GetBlogNotNull(id);
        Assert.Equal(newContent, dto2.Content);
    }

    [Fact]
    public async Task UpdateBlogContent_NotExists()
    {
        var id = Guid.NewGuid();
        var newContent = "New Content";
        
        var response = await _client.PutAsJsonAsync($"/blog/content/{id}", newContent);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBlog_Success()
    {
        var title = "Test Title";
        var description = "Test Description";
        var content = "Test Content";
        var author = "Test Author";
        
        var dto = await CreateBlog(title, description, content, author);
        var id = dto.Id;
        
        var response = await _client.DeleteAsync($"/blog/{id}");
        response.EnsureSuccessStatusCode();
        
        var response2 = await _client.GetAsync($"/blog/{id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response2.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBlog_NotExists()
    {
        var id = Guid.NewGuid();
        
        var response = await _client.DeleteAsync($"/blog/{id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<BlogDto> GetBlogNotNull(Guid id)
    {
        var blog = await GetBlog(id);
        Assert.NotNull(blog);
        return blog;
    }
    
    private async Task<BlogDto?> GetBlog(Guid id)
    {
        var response = await _client.GetAsync($"/blog/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BlogDto>();
    }

    private async Task<BlogDto> CreateBlog(string title, string description, string content, string author)
    {
        var response = await _client.PostAsJsonAsync("/blog", new CreateNewBlogRequest
        {
            Title = title,
            Description = description,
            Content = content,
            Author = author
        });
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<BlogDto>();
        Assert.NotNull(dto);
        return dto;
    }
}