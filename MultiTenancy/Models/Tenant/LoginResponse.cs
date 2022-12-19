using MultiTenancy.Entities;

namespace MultiTenancy.Models.Tenant;

public class LoginResponse
{
    public User? User { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; } = false;
}
