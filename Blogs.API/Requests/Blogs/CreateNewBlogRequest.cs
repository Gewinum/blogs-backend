namespace Blogs.API.Requests.Blogs;

public class CreateNewBlogRequest
{
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public required string Content { get; set; }
    
    public required string Author { get; set; }
}