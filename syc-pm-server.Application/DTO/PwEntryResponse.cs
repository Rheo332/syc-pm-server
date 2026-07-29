using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.DTO
{
    public class PwEntryResponse
    {
        //public List<EntryWithKey> PwEntries { get; set; } = [];

        public List<PwEntry> PwEntries { get; set; } = [];
    }

    /*public class EntryWithKey
    {
        public PwEntry PwEntry { get; set; } = null!;
        public string EncryptedEntryKey { get; set; } = null!;
    }*/
}
