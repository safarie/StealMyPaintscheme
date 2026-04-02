namespace StealMyPaintscheme.Api.Models;

public class Step
{
    public int Id { get; set; }
    public string Technique { get; set; } = string.Empty;
    
    // Relation to Paint
    public int PaintId { get; set; }
    public Paint Paint { get; set; } = null!;
    
    // Relation to PaintScheme
    public int PaintSchemeId { get; set; }
}
