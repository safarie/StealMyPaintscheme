using System.ComponentModel.DataAnnotations;

namespace StealMyPaintscheme.Api.Models;

public class PaintScheme
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    public List<string>? Tags { get; set; } = [];
    public List<Step> Steps { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relation to User
    public int UserId { get; set; }
    
}
