using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.Interfaces;

public interface IPwEntryRepository
{
    Task<List<PwEntry>?> GetAllPwEntries();
}

