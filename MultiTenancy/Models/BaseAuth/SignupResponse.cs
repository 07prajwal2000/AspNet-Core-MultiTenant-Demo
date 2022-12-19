namespace MultiTenancy.Models.BaseAuth;

public class SignupResponse
{
    public string TenantName { get; set; }
    public string RedirectUrl { get; set;}
    public string Email { get; set; }
}
