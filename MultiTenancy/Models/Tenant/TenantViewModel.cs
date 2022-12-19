using MultiTenancy.Entities;

namespace MultiTenancy.Models.Tenant;

public class TenantViewModel
{
    public User User { get; set; }
    public MultiTenancy.Entities.Tenant Tenant { get; set; }
    public List<User>? UserAccounts { get; set; }
}
