using Blogs.API.Requests.Blogs;
using Blogs.Application.Exceptions;
using Blogs.Domain.Dtos;
using Blogs.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blogs.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBlog(Guid id, IBlogService service)
    {
        try
        {
            return Ok(await service.GetBlogAsync(id));
        }
        catch (BlogNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody] CreateNewBlogRequest request, IBlogService service)
    {
        var dto = await service.CreateBlogAsync(request.Title, request.Description, request.Content, request.Author);
        return Ok(dto);
    }
    
    [HttpPut("title/{id:guid}")]
    public async Task<IActionResult> UpdateBlogTitle(Guid id, [FromBody] string title, IBlogService service)
    {
        try
        {
            await service.UpdateBlogTitleAsync(id, title);
            return Ok();
        }
        catch (BlogNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPut("description/{id:guid}")]
    public async Task<IActionResult> UpdateBlogDescription(Guid id, [FromBody] string description, IBlogService service)
    {
        try
        {
            await service.UpdateBlogDescriptionAsync(id, description);
            return Ok();
        }
        catch (BlogNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPut("content/{id:guid}")]
    public async Task<IActionResult> UpdateBlogContent(Guid id, [FromBody] string content, IBlogService service)
    {
        try
        {
            await service.UpdateBlogContentAsync(id, content);
            return Ok();
        }
        catch (BlogNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBlog(Guid id, IBlogService service)
    {
        try
        {
            await service.DeleteBlogAsync(id);
            return Ok();
        }
        catch (BlogNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}