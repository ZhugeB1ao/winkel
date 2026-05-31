using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using winkel.AppData;
using winkel.Models;

namespace winkel.Controllers;

[Route("Home")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}