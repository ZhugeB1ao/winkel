using Microsoft.AspNetCore.Mvc;

namespace winkel.Controllers;

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