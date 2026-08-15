using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;
using syc_pm_server.Infrastructure.Persistence;

namespace syc_pm_server.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid adminId, string username)
    {
        var admin = await _db.Users.FindAsync(adminId);
        if (admin == null || admin.Username != "admin") return false;

        var user = await _db.Users
            .Include(u => u.PwEntryAccesses)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return false;

        _db.PwEntryAccesses.RemoveRange(user.PwEntryAccesses);
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
