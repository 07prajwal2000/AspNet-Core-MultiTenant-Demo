using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Models.BaseAuth;
using MultiTenancy.Services;

namespace MultiTenancy.Controllers;

public class BaseAuthController : Controller
{
    private readonly BaseAuthServices services;

    public BaseAuthController(BaseAuthServices services)
    {
        this.services = services;
    }

    #region Page Views

    [HttpGet("/Signup")]
    public IActionResult Signup()
    {
        return View();
    }

    #endregion

    [HttpPost("Signup")]
    public async Task<IActionResult> Signup(SignupDto dto)
    {
        await services.SignupUser(dto);
        return RedirectToAction("Login", controllerName: dto.TenantName);
    }

}
