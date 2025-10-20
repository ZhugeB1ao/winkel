using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppData;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

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
    
    [HttpGet("{slug?}")]
    public IActionResult Index(string? slug)
    {
        // 1️⃣ Lấy toàn bộ categories
        var categories = _context.Categories.ToList();

        // 2️⃣ Nếu chưa chọn category → hiển thị tất cả
        if (string.IsNullOrEmpty(slug))
        {
            var viewAll = new ShopView
            {
                Categories = categories,
                Products = _context.Products.ToList(),
                CurrentCategory = null
            };
            return View(viewAll);
        }

        // 3️⃣ Nếu có slug → xác định category hiện tại
        var currentCategory = categories.FirstOrDefault(c => c.Slug == slug);

        if (currentCategory == null)
        {
            return NotFound();
        }

        // 4️⃣ Lấy sản phẩm thuộc category này và các category con
        var subCategoryIds = categories
            .Where(c => c.ParentId == currentCategory.Id)
            .Select(c => c.Id)
            .ToList();

        subCategoryIds.Add(currentCategory.Id);

        var products = _context.Products
            .Where(p => subCategoryIds.Contains(p.CategoryId))
            .ToList();

        var view = new ShopView
        {
            Categories = categories,
            Products = products,
            CurrentCategory = currentCategory
        };

        return View(view);
    }
    
    // 🛒 Trang chi tiết sản phẩm
    // [HttpGet("Product/{slug}")]
    // public IActionResult Product(string slug)
    // {
    //     var product = _context.Products
    //         .Include(p => p.Category)
    //         .FirstOrDefault(p => p.Slug == slug);
    //
    //     if (product == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var categories = _context.Categories.ToList();
    //
    //     var view = new ShopView
    //     {
    //         Categories = categories,
    //         CurrentProduct = product,
    //         CurrentCategory = product.Category
    //     };
    //
    //     return View("Index", view);
    // }
}