using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username) =>
        db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<bool> ExistsAsync(string username, string email) =>
        db.Users.AnyAsync(u => u.Username == username || u.Email == email);

    public async Task AddAsync(User user) => db.Users.Add(user);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
