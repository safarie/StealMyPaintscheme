using System.ComponentModel.DataAnnotations.Schema;

namespace StealMyPaintscheme.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;

    // Moment waarop de gebruiker heeft bevestigd het privacybeleid te hebben gelezen (verantwoordingsplicht, AVG art. 5 lid 2)
    public DateTime? PrivacyPolicyAcceptedAt { get; set; }

    // Wordt alleen bij registratie meegestuurd door de client, niet opgeslagen
    [NotMapped]
    public bool AcceptPrivacyPolicy { get; set; }

    public List<PaintScheme> PaintSchemes { get; set; } = [];
    public List<InventoryItem> Inventory { get; set; } = [];
}
