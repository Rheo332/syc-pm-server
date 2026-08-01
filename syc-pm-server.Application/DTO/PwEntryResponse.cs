using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.DTO
{
    public class PwEntryResponse
    {
        public List<PwEntry> PwEntries { get; set; } = [];
    }
}
