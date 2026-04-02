using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/hello-world", () => "Hello world!");

// Users
app.MapPost("/users", async (AppDbContext db, User user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
}).WithName("CreateUser");

app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.ToListAsync()).WithName("GetUsers");

// Paints
app.MapPost("/paints", async (AppDbContext db, Paint paint) =>
{
    db.Paints.Add(paint);
    await db.SaveChangesAsync();
    return Results.Created($"/paints/{paint.Id}", paint);
}).WithName("CreatePaint");

app.MapGet("/paints", async (AppDbContext db) =>
    await db.Paints.ToListAsync()).WithName("GetPaints");

// PaintSchemes
app.MapPost("/paint-schemes", async (AppDbContext db, PaintScheme paintScheme) =>
{
    paintScheme.CreatedAt = DateTime.UtcNow;
    db.PaintSchemes.Add(paintScheme);
    await db.SaveChangesAsync();
    return Results.Created($"/paint-schemes/{paintScheme.Id}", paintScheme);
}).WithName("CreatePaintScheme");

app.MapGet("/paint-schemes", async (AppDbContext db) =>
    await db.PaintSchemes.Include(ps => ps.Steps).ThenInclude(s => s.Paint).ToListAsync()).WithName("GetPaintSchemes");

// Steps
app.MapPost("/steps", async (AppDbContext db, Step step) =>
{
    db.Steps.Add(step);
    await db.SaveChangesAsync();
    return Results.Created($"/steps/{step.Id}", step);
}).WithName("CreateStep");

// InventoryItems
app.MapPost("/inventory-items", async (AppDbContext db, InventoryItem inventoryItem) =>
{
    db.InventoryItems.Add(inventoryItem);
    await db.SaveChangesAsync();
    return Results.Created($"/inventory-items/{inventoryItem.Id}", inventoryItem);
}).WithName("CreateInventoryItem");

app.Run();
