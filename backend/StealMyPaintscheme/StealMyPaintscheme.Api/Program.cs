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
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
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

builder.Services.AddAuthorization();

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
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/hello-world", () => "Hello world!");

// Login
app.MapPost("/login", async (AppDbContext db, User loginUser, IConfiguration config) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);
    if (user is null) return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userId", user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(3),
        signingCredentials: creds
    );

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    });
}).WithName("Login");

// Users
app.MapPost("/users", async (AppDbContext db, User user) =>
{
    var existingUser = await db.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email);
    if (existingUser) return Results.BadRequest("Gebruikersnaam of e-mail is al in gebruik.");

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
}).WithName("CreateUser");

app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.ToListAsync()).WithName("GetUsers").RequireAuthorization();

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
        
        // Zorg ervoor dat de steps ook gekoppeld zijn aan de juiste userId of andere velden indien nodig
        // In dit geval worden ze als part of the aggregate opgeslagen.
        
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
        .OrderBy(ps => ps.UserId == currentUserId)
        .ThenByDescending(ps => ps.CreatedAt)
        .ToListAsync();
    return Results.Ok(schemes);
}).WithName("GetPaintSchemes").RequireAuthorization();

app.MapDelete("/paint-schemes/{id}", async (AppDbContext db, int id, ClaimsPrincipal userPrincipal, ILogger<Program> logger) =>
{
    var userIdClaim = userPrincipal.FindFirst("userId")?.Value;
    if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
    {
        logger.LogWarning("DeletePaintScheme: Geen userId claim gevonden voor id {Id}", id);
        return Results.Unauthorized();
    }

    logger.LogInformation("DeletePaintScheme: Poging om schema {Id} te verwijderen voor gebruiker {UserId}", id, userId);

    var scheme = await db.PaintSchemes
        .Include(ps => ps.Steps)
        .FirstOrDefaultAsync(ps => ps.Id == id && ps.UserId == userId);
        
    if (scheme == null)
    {
        var exists = await db.PaintSchemes.AnyAsync(ps => ps.Id == id);
        logger.LogWarning("DeletePaintScheme: Schema {Id} niet gevonden voor gebruiker {UserId}. Bestaat het schema überhaupt? {Exists}", id, userId, exists);
        return Results.NotFound();
    }

    db.PaintSchemes.Remove(scheme);
    await db.SaveChangesAsync();
    logger.LogInformation("DeletePaintScheme: Schema {Id} succesvol verwijderd", id);
    return Results.NoContent();
}).WithName("DeletePaintScheme").RequireAuthorization();

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

app.Run();
