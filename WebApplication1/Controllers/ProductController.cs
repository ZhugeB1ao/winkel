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
        
        var parent = product.Category.Parent ?? product.Category;

        var childIds = parent.Children.Select(c => c.Id).ToList();
        childIds.Add(parent.Id);
        
        var relatedProducts = _context.Products
            .Where(p => childIds.Contains(p.CategoryId) && p.Slug != slug)
            .Take(4)
            .ToList();

        foreach (var p in relatedProducts)
        {
            Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, CategoryId: {p.CategoryId}");
        }
        
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