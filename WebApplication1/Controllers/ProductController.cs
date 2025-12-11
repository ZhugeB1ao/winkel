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

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Detail/{id}")]
    public IActionResult Detail(int id)
    {
        Product product = new Product();
        product = _context.Products.Find(id);
        return View(product);
    }
    
    [HttpGet("ListPro/{id}")]
    public IActionResult ListPro(int id)
    {
        // Get child Category
        var childCategoryIds = _context.Categories
            .Where(c => c.ParentId == id)
            .Select(c => c.Id)
            .ToList();
        
        childCategoryIds.Add(id);

        // Get all Product of current Category or child Category
        var products = _context.Products
            .Where(p => childCategoryIds.Contains(p.CategoryId))
            .ToList();

        return View("../Shop/Index", products);
    }
}