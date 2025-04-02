using System.Data.Common;
using Blogs.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Infrastructure.Database;

public class ApplicationDbContext: DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    { }
}