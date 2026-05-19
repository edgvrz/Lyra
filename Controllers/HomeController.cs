using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lyra.Models;
using Lyra.Enums;

namespace Lyra.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(UserRoles.Admin))
                    return RedirectToAction("Dashboard", "Admin");
                if (User.IsInRole(UserRoles.Tienda))
                    return RedirectToAction("Dashboard", "Tienda");
                if (User.IsInRole(UserRoles.Cliente))
                    return RedirectToAction("Dashboard", "Cliente");
            }
            return View();
    }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
    }

