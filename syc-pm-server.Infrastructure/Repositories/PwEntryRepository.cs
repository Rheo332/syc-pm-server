using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;
using syc_pm_server.Infrastructure.Persistence;

namespace syc_pm_server.Infrastructure.Repositories;

public class PwEntryRepository : IPwEntryRepository
{
    private readonly AppDbContext _db;

    public PwEntryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PwEntryAccess>> GetUserEntriesAsync(string username)
    {
        return await _db.PwEntryAccesses
            .Include(ea => ea.PwEntry)
            .Where(ea => ea.User.Username == username)
            .ToListAsync();
    }
}
