using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
