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

    public async Task<List<PwEntry>?> GetAllPwEntries()
    {
        return await _db.PwEntries.ToListAsync();
    }
}
