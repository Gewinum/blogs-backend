namespace Blogs.Application.Exceptions;

public class BlogNotFoundException : ApplicationException
{
    public BlogNotFoundException(Guid id) : base("Blog with ID {id} was not found.", 40400)
    {
        
    }
}