using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<List<string>> GetImageUrlsOnlyUsedByUserAsync(int userId);
    Task DeleteWithAllDataAsync(int userId);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string username, string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<string>> GetImageUrlsOnlyUsedByUserAsync(int userId)
    {
        // Afbeeldingen die ook in (gestolen) schema's van andere gebruikers staan blijven bewaard
        return await _db.PaintSchemes
            .Where(ps => ps.UserId == userId && ps.ImageUrl != null)
            .Where(ps => !_db.PaintSchemes.Any(other => other.UserId != userId && other.ImageUrl == ps.ImageUrl))
            .Select(ps => ps.ImageUrl!)
            .Distinct()
            .ToListAsync();
    }

    public async Task DeleteWithAllDataAsync(int userId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        var userPaintIds = _db.Paints.Where(p => p.UserId == userId).Select(p => p.Id);
        var userSchemeIds = _db.PaintSchemes.Where(ps => ps.UserId == userId).Select(ps => ps.Id);

        // Stappen van andere gebruikers mogen niet meer naar verf van deze gebruiker verwijzen
        await _db.Steps
            .Where(s => s.PaintId != null && userPaintIds.Contains(s.PaintId.Value))
            .ExecuteUpdateAsync(set => set.SetProperty(s => s.PaintId, (int?)null));

        await _db.Steps
            .Where(s => s.PaintSchemeId != null && userSchemeIds.Contains(s.PaintSchemeId.Value))
            .ExecuteDeleteAsync();
        await _db.PaintSchemes.Where(ps => ps.UserId == userId).ExecuteDeleteAsync();
        await _db.InventoryItems.Where(i => i.UserId == userId).ExecuteDeleteAsync();
        await _db.Paints.Where(p => p.UserId == userId).ExecuteDeleteAsync();
        await _db.Users.Where(u => u.Id == userId).ExecuteDeleteAsync();

        await transaction.CommitAsync();
    }
}
