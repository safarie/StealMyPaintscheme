namespace StealMyPaintscheme.Api.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    
    // Relation to Paint
    public int PaintId { get; set; }
    public Paint Paint { get; set; } = null!;
    
    // Relation to User
    public int UserId { get; set; }
}
