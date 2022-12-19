using Microsoft.EntityFrameworkCore;
using MultiTenancy.Entities;

namespace MultiTenancy.Data;

public class DataContext : DbContext
{
    private readonly string connectionString;

    public DataContext(DbContextOptions<DataContext> o, string connectionString) : base(o)
	{
        this.connectionString = connectionString;
    }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
