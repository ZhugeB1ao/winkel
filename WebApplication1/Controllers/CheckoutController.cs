using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("Checkout")]
public class CheckoutController : Controller
{
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
}