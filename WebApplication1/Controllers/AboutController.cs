using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("About")]
public class AboutController : Controller
{
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
}