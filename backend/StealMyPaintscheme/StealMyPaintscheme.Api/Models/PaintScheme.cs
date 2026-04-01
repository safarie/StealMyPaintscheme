namespace StealMyPaintscheme.Api.Models;

public class PaintScheme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Step> Steps { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relation to User
    public int UserId { get; set; }
}
