using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public SubscriptionType SubscriptionType { get; set; }

    [ForeignKey("fk_TenantId")]
    public Guid? TenantId { get; set; }

    public Tenant Tenant { get; set; }
}

public enum SubscriptionType
{
    Free,
    Pro,
    Enterprise,
}