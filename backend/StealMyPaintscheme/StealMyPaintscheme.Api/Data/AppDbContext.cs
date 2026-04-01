using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PaintScheme> PaintSchemes => Set<PaintScheme>();
    public DbSet<Paint> Paints => Set<Paint>();
    public DbSet<Step> Steps => Set<Step>();
    public DbSet<User> Users => Set<User>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
}
