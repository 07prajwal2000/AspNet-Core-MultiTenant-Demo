using Microsoft.EntityFrameworkCore;
using MultiTenancy.Entities;

namespace MultiTenancy.Data;

//public class GlobalDataContext : DbContext
//{
//    private readonly string ConnectionString;

//    public GlobalDataContext(DbContextOptions<GlobalDataContext> o, string tenant = "Data Source=global.db", bool shared = true) : base(o)
//    {
//        if (!shared)
//        {
//            var path = Path.Combine(Environment.CurrentDirectory, $"Tenant-{tenant}", "database");
//            var info = Directory.CreateDirectory(path);
//            ConnectionString = $"Data Source={Path.Combine(path, "database.db")}";
//            return;
//        }
//        ConnectionString = tenant;
//    }

//    public DbSet<Client> Clients { get; set; }
//    public DbSet<Tenant> Tenants { get; set; }

//    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        base.OnConfiguring(optionsBuilder);
//        optionsBuilder.UseSqlite(ConnectionString);
//    }
//}

public class GlobalDataContext : DbContext
{

    public GlobalDataContext(DbContextOptions<GlobalDataContext> o) : base(o) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
}