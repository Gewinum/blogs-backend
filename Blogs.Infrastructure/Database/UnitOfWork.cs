using System.Data.Common;
using Blogs.Domain.Interfaces.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace Blogs.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction = null;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction has already been started");
        }
        
        _transaction = await _context.Database.BeginTransactionAsync(); 
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("Transaction has not been started");
        }
        
        await _transaction.CommitAsync();
        _transaction.Dispose();
        _transaction = null;
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("Transaction has not been started");
        }
        
        await _transaction.RollbackAsync();
        _transaction.Dispose();
        _transaction = null;
    }
}