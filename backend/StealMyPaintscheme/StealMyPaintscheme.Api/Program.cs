using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Staat alle origins toe, ook met credentials indien nodig
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("isAdmin", "true"));
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Automatisch de frontend starten in een nieuw terminalvenster
    try
    {
        // Bepaal het pad naar de frontend map (relatief aan de backend API map)
        var frontendPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "..", "frontend"));
        
        if (Directory.Exists(frontendPath))
        {
            Console.WriteLine($"[INFO] Starten van frontend in: {frontendPath}");
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c npm start",
                WorkingDirectory = frontendPath,
                UseShellExecute = true, // Opent een nieuw terminalvenster
                CreateNoWindow = false
            };
            
            Process.Start(startInfo);
        }
        else
        {
            Console.WriteLine($"[WAARSCHUWING] Frontend map niet gevonden op: {frontendPath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FOUT] Kon de frontend niet automatisch starten: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/hello-world", () => "Hello world!");

// Login
app.MapPost("/login", async (AppDbContext db, User loginUser, IConfiguration config, ILogger<Program> logger) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username);
    
    bool isValid = false;
    try 
    {
        isValid = user is not null && BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);
    }
    catch (BCrypt.Net.SaltParseException)
    {
        logger.LogWarning("Ongeldige password hash gevonden voor gebruiker {Username}", loginUser.Username);
    }

    if (!isValid || user is null) 
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userId", user.Id.ToString()),
        new Claim("isAdmin", user.IsAdmin.ToString().ToLower())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claimsIdentity = new ClaimsIdentity(claims);
    logger.LogInformation("Login: Claims voor gebruiker {Username}: {Claims}", user.Username, string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(3),
        signingCredentials: creds
    );

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        isAdmin = user.IsAdmin
    });
}).WithName("Login");

// Users
app.MapPost("/users", async (AppDbContext db, User user) =>
{
    var existingUser = await db.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email);
    if (existingUser) return Results.BadRequest("Gebruikersnaam of e-mail is al in gebruik.");

    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    user.IsAdmin = false; // Voorkom dat gebruikers zichzelf admin maken bij registratie
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
}).WithName("CreateUser");


// Paints
app.MapPost("/paints", async (AppDbContext db, Paint paint, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var existingPaint = await db.Paints.FirstOrDefaultAsync(p => p.Name == paint.Name && p.Maker == paint.Maker && p.UserId == userId);
    if (existingPaint != null)
    {
        return Results.Ok(existingPaint);
    }
    
    paint.UserId = userId;
    db.Paints.Add(paint);
    await db.SaveChangesAsync();
    return Results.Created($"/paints/{paint.Id}", paint);
}).WithName("CreatePaint").RequireAuthorization();

app.MapGet("/paints", async (AppDbContext db, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var paints = await db.Paints.Where(p => p.UserId == userId).ToListAsync();
    return Results.Ok(paints);
}).WithName("GetPaints").RequireAuthorization();

// PaintSchemes
app.MapPost("/paint-schemes", async (AppDbContext db, PaintScheme paintScheme, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    try 
    {
        paintScheme.UserId = userId;
        paintScheme.CreatedAt = DateTime.UtcNow;
        
        // Valideer PaintId's in steps om FK violations te voorkomen
        if (paintScheme.Steps != null)
        {
            var validPaintIds = await db.Paints.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
            var globalPaintIds = await db.GlobalPaints.Select(p => p.Id).ToListAsync();

            foreach (var step in paintScheme.Steps)
            {
                if (step.PaintId.HasValue)
                {
                    // Als de PaintId niet in de eigen paints EN niet in de globale paints zit, zet hem op null
                    if (!validPaintIds.Contains(step.PaintId.Value) && !globalPaintIds.Contains(step.PaintId.Value))
                    {
                        step.PaintId = null;
                        step.Paint = null;
                    }
                }
            }
        }
        
        db.PaintSchemes.Add(paintScheme);
        await db.SaveChangesAsync();
        return Results.Created($"/paint-schemes/{paintScheme.Id}", paintScheme);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Error saving paint scheme: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
        }
        return Results.Problem("Er is een fout opgetreden bij het opslaan van het verfschema.");
    }
}).WithName("CreatePaintScheme").RequireAuthorization();

app.MapGet("/paint-schemes", async (AppDbContext db, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    int? currentUserId = int.TryParse(userIdClaim, out var id) ? id : null;

    var schemes = await db.PaintSchemes
        .Include(ps => ps.Steps)
        .ThenInclude(s => s.Paint)
        .ToListAsync();

    var orderedSchemes = schemes
        .OrderByDescending(ps => ps.UserId == currentUserId)
        .ThenByDescending(ps => ps.CreatedAt)
        .ToList();

    return Results.Ok(orderedSchemes);
}).WithName("GetPaintSchemes");

app.MapDelete("/paint-schemes/{id}", async (AppDbContext db, int id, ClaimsPrincipal userPrincipal, ILogger<Program> logger) =>
{
    foreach (var claim in userPrincipal.Claims)
    {
        logger.LogInformation("DeletePaintScheme: Claim gevonden - Type: {Type}, Value: {Value}", claim.Type, claim.Value);
    }

    var isAdmin = userPrincipal.HasClaim("isAdmin", "true") || 
                  userPrincipal.HasClaim(c => c.Type.EndsWith("isAdmin") && c.Value.ToLower() == "true");

    var userIdClaim = userPrincipal.FindFirst("userId")?.Value ?? 
                      userPrincipal.FindFirst(c => c.Type.EndsWith("userId"))?.Value;

    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        if (isAdmin)
        {
            logger.LogInformation("DeletePaintScheme: Geen geldige userId claim gevonden voor admin, maar gaat door.");
            userId = 0; // Dummy value voor admin
        }
        else
        {
            logger.LogWarning("DeletePaintScheme: Geen userId claim gevonden voor id {Id}", id);
            return Results.Unauthorized();
        }
    }

    logger.LogInformation("DeletePaintScheme: Poging om schema {Id} te verwijderen door gebruiker {UserId} (Admin: {IsAdmin})", id, userId, isAdmin);

    var query = db.PaintSchemes.Include(ps => ps.Steps).AsQueryable();
    
    // Als de gebruiker geen admin is, mag hij alleen zijn eigen schema's verwijderen
    if (!isAdmin)
    {
        query = query.Where(ps => ps.UserId == userId);
    }
    else 
    {
        logger.LogInformation("DeletePaintScheme: Gebruiker is admin, UserId filter overgeslagen.");
    }

    var scheme = await query.FirstOrDefaultAsync(ps => ps.Id == id);
        
    if (scheme == null)
    {
        var exists = await db.PaintSchemes.AnyAsync(ps => ps.Id == id);
        logger.LogWarning("DeletePaintScheme: Schema {Id} niet gevonden of niet toegankelijk voor gebruiker {UserId}. Bestaat het schema überhaupt? {Exists}", id, userId, exists);
        return Results.NotFound();
    }

    db.PaintSchemes.Remove(scheme);
    await db.SaveChangesAsync();
    logger.LogInformation("DeletePaintScheme: Schema {Id} succesvol verwijderd", id);
    return Results.NoContent();
}).WithName("DeletePaintScheme").RequireAuthorization();

app.MapPut("/paint-schemes/{id}", async (AppDbContext db, int id, PaintScheme updatedScheme, ClaimsPrincipal userPrincipal, ILogger<Program> logger) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var scheme = await db.PaintSchemes
        .Include(ps => ps.Steps)
        .FirstOrDefaultAsync(ps => ps.Id == id && ps.UserId == userId);

    if (scheme == null) return Results.NotFound();

    scheme.Name = updatedScheme.Name;
    scheme.Description = updatedScheme.Description;
    scheme.Tags = updatedScheme.Tags;
    scheme.ImageUrl = updatedScheme.ImageUrl;
    
    // Update steps
    db.Steps.RemoveRange(scheme.Steps);
    
    // Valideer PaintId's in nieuwe stappen om FK violations te voorkomen
    if (updatedScheme.Steps != null)
    {
        var validPaintIds = await db.Paints.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
        var globalPaintIds = await db.GlobalPaints.Select(p => p.Id).ToListAsync();

        foreach (var step in updatedScheme.Steps)
        {
            if (step.PaintId.HasValue)
            {
                if (!validPaintIds.Contains(step.PaintId.Value) && !globalPaintIds.Contains(step.PaintId.Value))
                {
                    step.PaintId = null;
                    step.Paint = null;
                }
            }
            step.PaintSchemeId = scheme.Id;
        }
    }
    scheme.Steps = updatedScheme.Steps;

    try 
    {
        await db.SaveChangesAsync();
        return Results.Ok(scheme);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Fout bij bijwerken: {ex.Message}");
    }
}).WithName("UpdatePaintScheme").RequireAuthorization();

// Upload Image
app.MapPost("/upload", async (IFormFile file, IWebHostEnvironment env, ILogger<Program> logger) =>
{
    try
    {
        if (file == null || file.Length == 0)
        {
            logger.LogWarning("UploadImage: Geen bestand geselecteerd of bestand is leeg.");
            return Results.BadRequest("Geen bestand geselecteerd.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            logger.LogWarning("UploadImage: Geblokkeerde bestandsextensie {Extension}", extension);
            return Results.BadRequest("Alleen afbeeldingen toegestaan (.jpg, .jpeg, .png, .webp, .gif).");
        }

        var header = new byte[12];
        using (var peek = file.OpenReadStream())
        {
            await peek.ReadAsync(header, 0, header.Length);
        }

        bool validMagic =
            // JPEG: FF D8 FF
            (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) ||
            // PNG: 89 50 4E 47 0D 0A 1A 0A
            (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) ||
            // GIF: GIF87a / GIF89a
            (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) ||
            // WebP: RIFF????WEBP
            (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
             header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50);

        if (!validMagic)
        {
            logger.LogWarning("UploadImage: Magic bytes komen niet overeen met een toegestaan afbeeldingsformaat.");
            return Results.BadRequest("Bestandsinhoud komt niet overeen met een afbeelding.");
        }

        if (string.IsNullOrEmpty(env.WebRootPath))
        {
            logger.LogError("UploadImage: env.WebRootPath is null of leeg. Zorg ervoor dat wwwroot bestaat.");
            return Results.Problem("Server configuratie fout: wwwroot niet gevonden.");
        }

        var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            logger.LogInformation("UploadImage: Aanmaken van uploads map: {Path}", uploadsFolder);
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        logger.LogInformation("UploadImage: Bestand opslaan naar {Path}", filePath);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"/uploads/{fileName}";
        logger.LogInformation("UploadImage: Bestand succesvol geüpload. URL: {Url}", imageUrl);
        return Results.Ok(new { imageUrl });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "UploadImage: Fout bij het uploaden van bestand.");
        return Results.Problem($"Interne serverfout: {ex.Message}");
    }
}).WithName("UploadImage").RequireAuthorization().DisableAntiforgery();

// Steps
app.MapPost("/steps", async (AppDbContext db, Step step) =>
{
    db.Steps.Add(step);
    await db.SaveChangesAsync();
    return Results.Created($"/steps/{step.Id}", step);
}).WithName("CreateStep").RequireAuthorization();

// InventoryItems
app.MapPost("/inventory-items", async (AppDbContext db, InventoryItem inventoryItem, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }
    
    // Check if the user already has this paint in their inventory
    var existingItem = await db.InventoryItems
        .FirstOrDefaultAsync(i => i.UserId == userId && i.PaintId == inventoryItem.PaintId);

    if (existingItem != null)
    {
        existingItem.Quantity += inventoryItem.Quantity;
        await db.SaveChangesAsync();
        return Results.Ok(existingItem);
    }
    
    inventoryItem.UserId = userId;
    db.InventoryItems.Add(inventoryItem);
    await db.SaveChangesAsync();
    return Results.Created($"/inventory-items/{inventoryItem.Id}", inventoryItem);
}).WithName("CreateInventoryItem").RequireAuthorization();

app.MapGet("/inventory-items", async (AppDbContext db, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var items = await db.InventoryItems
        .Include(i => i.Paint)
        .Where(i => i.UserId == userId)
        .ToListAsync();
    return Results.Ok(items);
}).WithName("GetInventoryItems").RequireAuthorization();

app.MapDelete("/inventory-items/{id}", async (AppDbContext db, int id, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var item = await db.InventoryItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    if (item == null) return Results.NotFound();

    db.InventoryItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("DeleteInventoryItem").RequireAuthorization();

app.MapPut("/inventory-items/{id}", async (AppDbContext db, int id, InventoryItem updatedItem, ClaimsPrincipal userPrincipal) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var item = await db.InventoryItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    if (item == null) return Results.NotFound();

    item.Quantity = updatedItem.Quantity;
    
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).WithName("UpdateInventoryItem").RequireAuthorization();

// GlobalPaints
app.MapGet("/global-paints", async (AppDbContext db) =>
    await db.GlobalPaints.ToListAsync()).WithName("GetGlobalPaints");

app.MapPost("/global-paints/import", async (AppDbContext db, List<GlobalPaint> paints) =>
{
    foreach (var paint in paints)
    {
        var exists = await db.GlobalPaints.AnyAsync(p => p.Name == paint.Name && p.Maker == paint.Maker);
        if (!exists)
        {
            db.GlobalPaints.Add(paint);
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok(new { message = $"{paints.Count} verfjes verwerkt." });
}).WithName("ImportGlobalPaints").RequireAuthorization("AdminOnly");

app.Run();
