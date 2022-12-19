namespace MultiTenancy.Models.Tenant;

public record AddTenantUserDto(string Email, string Name, string Password, string Roles);
