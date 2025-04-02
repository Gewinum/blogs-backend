using System.ComponentModel.DataAnnotations;
using Blogs.Domain.Interfaces.Models;

namespace Blogs.Domain.Models;

public abstract class BaseModel : IModel
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}