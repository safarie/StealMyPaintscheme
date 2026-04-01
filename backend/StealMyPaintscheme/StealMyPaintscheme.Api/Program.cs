using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

app.MapPost("/paints", async (AppDbContext db, Paint paint) =>
    {
        db.Paints.Add(paint);
        await db.SaveChangesAsync();
        return Results.Created($"/paints/{paint.Id}", paint);
    })
    .WithName("CreatePaint");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}