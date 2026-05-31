using Microsoft.AspNetCore.Mvc;

namespace winkel.Controllers;

[Route("Contact")]
public class ContactController : Controller
{
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
}