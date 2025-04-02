using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.Models;

public class Blog : BaseModel
{
    [MaxLength(255)]
    public required string Author { get; set; }
    
    [MaxLength(255)]
    public required string Title { get; set; }
    
    [MaxLength(400)]
    public required string Description { get; set; }
    
    [MaxLength(10000)]
    public required string Content { get; set; }
}