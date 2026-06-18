using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : BaseController
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;
    /// <summary>
    /// Initialiseert een nieuwe instantie van de <see cref="UploadController"/> klasse.
    /// </summary>
    /// <param name="env">De webhostingomgeving informatie.</param>
    /// <param name="logger">De logger voor het vastleggen van gebeurtenissen.</param>
    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Uploadt een afbeelding naar de server.
    /// </summary>
    /// <param name="file">Het bestand dat geüpload moet worden.</param>
    /// <returns>Een actieresultaat met de URL van de geüploade afbeelding of een foutmelding.</returns>
    [HttpPost]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            // Controleer of er een bestand is meegegeven en of het niet leeg is.
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadImage: Geen bestand geselecteerd of bestand is leeg.");
                return BadRequest("Geen bestand geselecteerd.");
            }

            // Definieer de toegestane bestandsextensies.
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            // Controleer of de extensie van het bestand is toegestaan.
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("UploadImage: Geblokkeerde bestandsextensie {Extension}", extension);
                return BadRequest("Alleen afbeeldingen toegestaan (.jpg, .jpeg, .png, .webp, .gif).");
            }

            // Lees de eerste 12 bytes van het bestand om de "magic bytes" te controleren (veiligheidscheck).
            var header = new byte[12];
            using (var peek = file.OpenReadStream())
            {
                await peek.ReadAsync(header, 0, header.Length);
            }

            // Valideer de magic bytes tegen bekende afbeeldingsformaten (JPEG, PNG, GIF, WEBP).
            bool validMagic =
                (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) || // JPEG
                (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) || // PNG
                (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) || // GIF
                (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 && // RIFF (WEBP)
                 header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50); // WEBP

            if (!validMagic)
            {
                _logger.LogWarning("UploadImage: Magic bytes komen niet overeen met een toegestaan afbeeldingsformaat.");
                return BadRequest("Bestandsinhoud komt niet overeen met een afbeelding.");
            }

            // Controleer of de fysieke pad naar de wwwroot is geconfigureerd.
            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                _logger.LogError("UploadImage: env.WebRootPath is null of leeg. Zorg ervoor dat wwwroot bestaat.");
                return Problem("Server configuratie fout: wwwroot niet gevonden.");
            }

            // Bepaal de map waar de uploads moeten komen en maak deze aan als ze nog niet bestaat.
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                _logger.LogInformation("UploadImage: Aanmaken van uploads map: {Path}", uploadsFolder);
                Directory.CreateDirectory(uploadsFolder);
            }

            // Genereer een unieke bestandsnaam met een GUID om overschrijven te voorkomen.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Sla het bestand fysiek op de server op.
            _logger.LogInformation("UploadImage: Bestand opslaan naar {Path}", filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retourneer de relatieve URL van de opgeslagen afbeelding.
            var imageUrl = $"/uploads/{fileName}";
            _logger.LogInformation("UploadImage: Bestand succesvol geüpload. URL: {Url}", imageUrl);
            return Ok(new { imageUrl });
        }
        catch (Exception ex)
        {
            // Log de fout en retourneer een serverfout.
            _logger.LogError(ex, "UploadImage: Fout bij het uploaden van bestand.");
            return Problem($"Interne serverfout: {ex.Message}");
        }
    }
}
