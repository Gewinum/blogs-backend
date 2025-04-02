namespace Blogs.Domain.Interfaces.Database;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
    
    public Task BeginTransactionAsync();
    
    public Task CommitTransactionAsync();
    
    public Task RollbackTransactionAsync();
}