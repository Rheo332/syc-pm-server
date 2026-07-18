using Microsoft.EntityFrameworkCore;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PwEntry> PwEntries => Set<PwEntry>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PwEntryAccess> PwEntryAccesses => Set<PwEntryAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PwEntryAccess>()
            .HasKey(ea => new { ea.PwEntryId, ea.UserId });

        modelBuilder.Entity<PwEntryAccess>()
            .HasOne(ea => ea.User)
            .WithMany(u => u.PwEntryAccesses)
            .HasForeignKey(ea => ea.UserId);

        modelBuilder.Entity<PwEntryAccess>()
            .HasOne(ea => ea.PwEntry)
            .WithMany(e => e.AuthorizedUsers)
            .HasForeignKey(ea => ea.PwEntryId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}