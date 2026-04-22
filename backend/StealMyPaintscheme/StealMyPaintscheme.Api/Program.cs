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
app.MapPost("/paints", async (AppDbContext db, Paint paint) =>
{
    db.Paints.Add(paint);
    await db.SaveChangesAsync();
    return Results.Created($"/paints/{paint.Id}", paint);
}).WithName("CreatePaint").RequireAuthorization();

app.MapGet("/paints", async (AppDbContext db) =>
    await db.Paints.ToListAsync()).WithName("GetPaints").RequireAuthorization();

// PaintSchemes
app.MapPost("/paint-schemes", async (AppDbContext db, PaintScheme paintScheme) =>
{
    paintScheme.CreatedAt = DateTime.UtcNow;
    db.PaintSchemes.Add(paintScheme);
    await db.SaveChangesAsync();
    return Results.Created($"/paint-schemes/{paintScheme.Id}", paintScheme);
}).WithName("CreatePaintScheme").RequireAuthorization();

app.MapGet("/paint-schemes", async (AppDbContext db) =>
    await db.PaintSchemes.Include(ps => ps.Steps).ThenInclude(s => s.Paint).ToListAsync()).WithName("GetPaintSchemes").RequireAuthorization();

// Steps
app.MapPost("/steps", async (AppDbContext db, Step step) =>
{
    db.Steps.Add(step);
    await db.SaveChangesAsync();
    return Results.Created($"/steps/{step.Id}", step);
}).WithName("CreateStep").RequireAuthorization();

// InventoryItems
app.MapPost("/inventory-items", async (AppDbContext db, InventoryItem inventoryItem) =>
{
    db.InventoryItems.Add(inventoryItem);
    await db.SaveChangesAsync();
    return Results.Created($"/inventory-items/{inventoryItem.Id}", inventoryItem);
}).WithName("CreateInventoryItem").RequireAuthorization();

app.Run();
