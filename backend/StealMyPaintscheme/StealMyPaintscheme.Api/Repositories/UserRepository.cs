using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
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

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
