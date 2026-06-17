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
    public DbSet<Vault> Vaults => Set<Vault>();
    public DbSet<VaultMember> VaultMembers => Set<VaultMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VaultMember>()
            .HasKey(vm => new { vm.VaultId, vm.UserId });

        modelBuilder.Entity<VaultMember>()
            .HasOne(vm => vm.User)
            .WithMany(u => u.VaultMembers)
            .HasForeignKey(vm => vm.UserId);

        modelBuilder.Entity<VaultMember>()
            .HasOne(vm => vm.Vault)
            .WithMany(v => v.Members)
            .HasForeignKey(vm => vm.VaultId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}