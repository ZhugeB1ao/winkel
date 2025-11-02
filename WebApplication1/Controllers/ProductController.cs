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
        // Get product info and category that product belong to
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Slug == slug);

        if (product == null)
        {
            return NotFound();
        }

        var categories = _context.Categories.ToList();
        
        // Get parent group 
        var parent = product.Category.Parent ?? product.Category;

        // get id from other category in the same group
        var childIds = parent.Children.Select(c => c.Id).ToList();
        childIds.Add(parent.Id);
        
        // Get related product
        var relatedProducts = _context.Products
            .Where(p => childIds.Contains(p.CategoryId) && p.Slug != slug)
            .Take(4)
            .ToList();

        var view = new ShopView
        {
            Categories = categories,
            Products = relatedProducts,
            CurrentProduct = product,
                CurrentCategory = product.Category
        };

        return View(view);
    }
    
    
}