using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Entities;
using MultiTenancy.Models.Shared;
using MultiTenancy.Models.Tenant;
using MultiTenancy.Services;
using System.Security.Claims;

namespace MultiTenancy.Controllers;

[Route("{tenantName}")]
public class TenantController : Controller
{
    private readonly TenantServices tenantServices;

    public TenantController(TenantServices tenantServices)
    {
        this.tenantServices = tenantServices;
    }

    #region UI Actions

    [HttpGet]
    public async Task<IActionResult> Index(string tenantName)
    {
        var tenant = await tenantServices.GetTenantDetailsAsync(tenantName);
        if (tenant == null)
        {
            return NotFound(new
            {
                Message = "No tenant found"
            });
        }
        return View(tenant);
    }

    [HttpGet("Login")]
    public async Task<IActionResult> Login(string tenantName)
    {
        var tenant = await tenantServices.GetTenantDetailsAsync(tenantName);
        if (tenant == null)
        {
            return NotFound(new
            {
                Message = "No tenant found"
            });
        }
        return View(tenant);
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout(string tenantName)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction($"Login", controllerName: tenantName);
    }

    [HttpGet("AddUser")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddUser(string tenantName)
    {
        return View(model: tenantName);
    }

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile(string tenantName)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToAction(nameof(Login), controllerName: tenantName);
        }
        tenantName = User.Claims.FirstOrDefault(x => x.Type == "TenantName")!.Value;
        var tenant = await tenantServices.GetTenantDetailsAsync(tenantName);
        if (tenant == null)
        {
            return NotFound(new
            {
                Message = "No tenant found"
            });
        }
        var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
        var user = await tenantServices.GetUserAsync(tenantName, email);

        List<User> userAccounts = null;

        if (user?.IsAdmin ?? false)
        {
            var tenantId = User.Claims.FirstOrDefault(x => x.Type == "TenantId")!.Value;
            userAccounts = await tenantServices.GetTenantUsers(tenantName, Guid.Parse(tenantId));
        }

        return View(new TenantViewModel
        {
            Tenant = tenant,
            User = user!,
            UserAccounts = userAccounts
        });
    }

    #endregion

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string tenantName, LoginDto dto)
    {
        var tenant = await tenantServices.GetTenantDetailsAsync(tenantName);
        if (tenant == null)
        {
            return NotFound(new
            {
                Message = "No tenant found"
            });
        }

        var response = await tenantServices.LoginTenantUserAsync(tenantName, dto, tenant.Shared);
        if (!response.Success || response.User is null)
        {
            return BadRequest(new { response.Message });
        }

        await LoginAsync(new Claim[]
        {
            new Claim(ClaimTypes.Email, response.User.Email),
            new Claim(ClaimTypes.Role, response.User?.Roles ?? "default"),
            new Claim("TenantId", response.User.TenantId.ToString()),
            new Claim("TenantName", tenantName),
            new Claim("Shared", tenant.Shared.ToString()),
            new Claim(ClaimTypes.NameIdentifier, response.User.Name),
        });

        return RedirectToAction(nameof(Profile), controllerName: tenantName);
    }

    [HttpPost("AddUser")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddUser(string tenantName, AddTenantUserDto dto)
    {
        var tenantId = User.Claims.FirstOrDefault(x => x.Type == "TenantId")!.Value;
        await tenantServices.AddTenantUser(tenantName, tenantId, dto);
        return RedirectToAction(nameof(Profile), controllerName: tenantName);
    }

    private async Task LoginAsync(Claim[] claims)
    {
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity[]
        {
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
        }));
    }
}
