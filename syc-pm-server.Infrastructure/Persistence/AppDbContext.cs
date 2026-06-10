using Microsoft.EntityFrameworkCore;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}