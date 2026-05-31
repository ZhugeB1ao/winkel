using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using winkel.AppData;
using winkel.Models;

namespace winkel.Controllers;

[Route("/")]
[Route("Shop")]
public class ShopController : Controller
{
    private readonly ILogger<ShopController> _logger;
    private readonly AppDBContext _context;
    public ShopController(ILogger<ShopController> logger, AppDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        List<Product> products = _context.Products.ToList();
        
        var latestProducts = products
            .OrderByDescending(p => p.Id)
            .Take(4)
            .ToList();
            
        ViewBag.LatestProducts = latestProducts;
        return View(products);
    }
    
}