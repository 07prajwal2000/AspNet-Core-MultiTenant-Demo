using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Data;

public class DataContextFactory
{
    private readonly IConfiguration configuration;
    private bool shared;

    public DataContextFactory(IConfiguration configuration, IHttpContextAccessor contextAccessor)
    {
        this.configuration = configuration;
        shared = bool.Parse(contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "Shared")?.Value ?? "true");
    }

    public DataContext Create(string tenantName, bool? sharedDb = null)
    {
        if(sharedDb != null)
        {
            shared = (bool)sharedDb;
        }
        var context = new DataContext(new DbContextOptions<DataContext>(), configuration.GetConnectionString("Template")!.Replace("{DBNAME}", shared ? "multitenancy_shared_db" : tenantName));
        // TODO: filter data by tenant id
        return context;
    }
}
