using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

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