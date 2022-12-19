using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models.BaseAuth;

namespace MultiTenancy.Services;

public class BaseAuthServices
{
    private readonly GlobalDataContext dataContext;
    private readonly DataContextFactory factory;
    private readonly IConfiguration configuration;

    public BaseAuthServices(GlobalDataContext dataContext, DataContextFactory factory, IConfiguration configuration)
    {
        this.dataContext = dataContext;
        this.factory = factory;
        this.configuration = configuration;
    }

    public async Task<SignupResponse> SignupUser(SignupDto dto)
    {
        using var transaction = await dataContext.Database.BeginTransactionAsync(default);

        var connectionString = dto.SubscriptionType switch
        {
            SubscriptionType.Free => configuration.GetConnectionString("SharedDb"),
            SubscriptionType.Pro => configuration.GetConnectionString("Template")!.Replace("{DBNAME}", dto.TenantName),
            SubscriptionType.Enterprise => configuration.GetConnectionString("Template")!.Replace("{DBNAME}", dto.TenantName),
            _ => configuration.GetConnectionString("SharedDb"),
        };

        var tenant = new Tenant
        {
            Name = dto.TenantName,
            ConnectionString = connectionString,
            SubscriptionType = dto.SubscriptionType,
        };

        var tenantEntry = dataContext.Tenants.Add(tenant);

        await dataContext.SaveChangesAsync(default);

        var client = new Client
        {
            Email = dto.Email,
            Name = dto.Name,
            Password = dto.Password,
            SubscriptionType = dto.SubscriptionType,
            TenantId = tenantEntry.Entity.Id,
        };

        var clientEntry = dataContext.Clients.Add(client);

        await dataContext.SaveChangesAsync(default);

        using var ctx = factory.Create(connectionString, sharedDb: dto.SubscriptionType == SubscriptionType.Free);
        var created = await ctx.Database.EnsureCreatedAsync();
        ctx.Users.Add(new User
        {
            Email = dto.Email,
            IsAdmin = true,
            Name = dto.Name,
            Password = dto.Password,
            TenantId = tenantEntry.Entity.Id,
            Roles = "admin",
        });
        await ctx.SaveChangesAsync(default);

        await transaction.CommitAsync(default);
        return new SignupResponse
        {
            Email = dto.Email,
            RedirectUrl = "",
            TenantName = tenantEntry.Entity.Name,
        };
    }

}
