namespace Blogs.Domain.Interfaces.Exceptions;

public interface IIdentifiableException
{
    public int ErrorCode { get; }
}