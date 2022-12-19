using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models.Shared;
using MultiTenancy.Models.Tenant;

namespace MultiTenancy.Services;

public class TenantServices
{
    private readonly GlobalDataContext globalDataContext;
    private readonly DataContextFactory contextFactory;

    public TenantServices(GlobalDataContext globalDataContext, DataContextFactory contextFactory)
	{
        this.globalDataContext = globalDataContext;
        this.contextFactory = contextFactory;
    }


    public async Task<Tenant?> GetTenantDetailsAsync(string tenantName, bool includeTenant = false)
    {
        return await globalDataContext.Tenants.FirstOrDefaultAsync(x => x.Name == tenantName);
    }

    public async Task<List<User>> GetTenantUsers(string tenantName, Guid tenantId)
    {
        using var dataContext = contextFactory.Create(tenantName);
        return await dataContext.Users.Where(x => x.TenantId == tenantId).ToListAsync(default);
    }

    public async Task AddTenantUser(string tenantName, string tenantId, AddTenantUserDto dto)
    {
        var tId = Guid.Parse(tenantId);
        using var dataContext = contextFactory.Create(tenantName);
        dataContext.Users.Add(new User
        {
            TenantId = tId,
            Email = dto.Email,
            IsAdmin = dto.Roles.Contains("admin"),
            Name = dto.Name,
            Password = dto.Password,
            Roles = dto.Roles,
        });
        await dataContext.SaveChangesAsync(default);
    }

    public async Task<LoginResponse> LoginTenantUserAsync(string tenantName, LoginDto dto, bool shared)
    {
        using var dataContext = contextFactory.Create(tenantName, shared);
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null || user.Password != dto.Password)
        {
            return new LoginResponse
            {
                Message = user == null ? "No user found" : "Invalid password"
            };
        }

        return new LoginResponse
        {
            User = user,
            Success = true,
            Message = "Logged in successfully"
        };
    }

    public async Task<User?> GetUserAsync(string tenantName, string email)
    {
        using var dataContext = contextFactory.Create(tenantName);
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        return user;
    }

}
