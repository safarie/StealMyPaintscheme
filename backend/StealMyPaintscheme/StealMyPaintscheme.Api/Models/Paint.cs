namespace StealMyPaintscheme.Api.Models;

public class Paint
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Maker { get; set; }
}
