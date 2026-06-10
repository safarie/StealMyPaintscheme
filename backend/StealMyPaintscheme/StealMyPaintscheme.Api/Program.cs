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
using StealMyPaintscheme.Api.Repositories;
using StealMyPaintscheme.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaintRepository, PaintRepository>();
builder.Services.AddScoped<IPaintSchemeRepository, PaintSchemeRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IGlobalPaintRepository, GlobalPaintRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaintSchemeService, PaintSchemeService>();
builder.Services.AddScoped<IPaintService, PaintService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IGlobalPaintService, GlobalPaintService>();

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

// Database verbinding testen bij opstarten
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("[INFO] Database verbinding succesvol.");
        }
        else
        {
            Console.WriteLine("[WAARSCHUWING] Database is niet bereikbaar. Controleer of de database draait.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FOUT] Kritieke fout bij verbinden met database: {ex.Message}");
    }
}

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

app.MapControllers();

app.MapGet("/hello-world", () => "Hello world!");

app.Run();
