using MultiTenancy.Entities;

namespace MultiTenancy.Models.BaseAuth;

public record SignupDto(string Name, string Email, string Password, SubscriptionType SubscriptionType, string TenantName);
