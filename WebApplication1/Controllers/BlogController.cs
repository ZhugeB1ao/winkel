using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("Blog")]
public class BlogController : Controller
{
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("BlogSingle")]
    public IActionResult BlogSingle()
    {
        return View();
    }

}