using Blogs.Domain.Interfaces.Exceptions;

namespace Blogs.Application.Exceptions;

public abstract class ApplicationException : Exception, IIdentifiableException
{
    public int ErrorCode { get; }
    
    public ApplicationException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}