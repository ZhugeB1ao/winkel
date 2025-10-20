using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppData;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Route("Product")]
public class ProductController : Controller
{
    private readonly AppDBContext _context;

    public ProductController(AppDBContext context)
    {
        _context = context;
    }
    
    [HttpGet("{slug}")]
    public IActionResult Index(string slug)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Slug == slug);

        if (product == null)
        {
            return NotFound();
        }

        var categories = _context.Categories.ToList();

        var view = new ShopView
        {
            Categories = categories,
            CurrentProduct = product,
            CurrentCategory = product.Category
        };

        return View(view);
    }
}