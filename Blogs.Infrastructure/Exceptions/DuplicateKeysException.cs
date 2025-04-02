namespace Blogs.Infrastructure.Exceptions;

public class DuplicateKeysException : Exception
{
    public DuplicateKeysException() : base("Data with the same key(s) already exists")
    {
    }
}