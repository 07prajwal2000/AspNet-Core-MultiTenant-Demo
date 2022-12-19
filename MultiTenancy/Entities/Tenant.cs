namespace MultiTenancy.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public bool Shared => SubscriptionType == SubscriptionType.Free;
    public DateTime CreatedDate { get; set;} = DateTime.UtcNow;
}
