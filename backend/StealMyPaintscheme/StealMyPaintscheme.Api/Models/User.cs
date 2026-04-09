namespace StealMyPaintscheme.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public List<PaintScheme> PaintSchemes { get; set; } = [];
    public List<InventoryItem> Inventory { get; set; } = [];
}
