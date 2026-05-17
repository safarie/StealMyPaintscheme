using System.ComponentModel.DataAnnotations;

namespace StealMyPaintscheme.Api.Models;

public class Step
{
    public int Id { get; set; }
    
    [Required]
    public string Where { get; set; } = string.Empty;
    
    [Required]
    public string Colour { get; set; } = string.Empty;
    
    [Required]
    public string PaintingTechnique { get; set; } = string.Empty;
    
    // Relation to Paint (optional for now, as user requested)
    public int? PaintId { get; set; }
    public Paint? Paint { get; set; }
    
    // Relation to PaintScheme
    public int? PaintSchemeId { get; set; }
}
