namespace StealMyPaintscheme.Api.Endpoints;

public static class UploadEndpoints
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    public static void MapUploadEndpoints(this WebApplication app)
    {
        app.MapPost("/upload", async (IFormFile file, IWebHostEnvironment env, ILogger<Program> logger) =>
        {
            if (file is null || file.Length == 0)
                return Results.BadRequest("Geen bestand geselecteerd.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return Results.BadRequest("Alleen afbeeldingen toegestaan (.jpg, .jpeg, .png, .webp, .gif).");

            var header = new byte[12];
            using (var peek = file.OpenReadStream())
                await peek.ReadExactlyAsync(header, 0, header.Length);

            if (!HasValidImageMagicBytes(header))
                return Results.BadRequest("Bestandsinhoud komt niet overeen met een afbeelding.");

            if (string.IsNullOrEmpty(env.WebRootPath))
                return Results.Problem("Server configuratie fout: wwwroot niet gevonden.");

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            logger.LogInformation("Bestand geüpload: {FileName}", fileName);
            return Results.Ok(new { imageUrl = $"/uploads/{fileName}" });
        }).WithName("UploadImage").RequireAuthorization().DisableAntiforgery();
    }

    private static bool HasValidImageMagicBytes(byte[] header) =>
        (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) ||
        (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) ||
        (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) ||
        (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
         header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50);
}
