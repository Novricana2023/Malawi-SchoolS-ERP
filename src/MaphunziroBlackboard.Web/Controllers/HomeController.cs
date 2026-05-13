using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaphunziroBlackboard.Application.Interfaces;
using MaphunziroBlackboard.Web.Models;
using System.Diagnostics;

namespace MaphunziroBlackboard.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IUserService userService, ILogger<HomeController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [Authorize]
    public async Task<IActionResult> About()
    {
        ViewData["Title"] = "About Maphunziro Board";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult PageNotFound()
    {
        return View();
    }
}
