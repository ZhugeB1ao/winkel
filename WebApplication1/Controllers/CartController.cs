using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("Cart")]
public class CartController : Controller
{
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
}