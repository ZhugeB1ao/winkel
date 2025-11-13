using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppData;

namespace WebApplication1.Components;

public class CategoryViewComponent : ViewComponent
{
    private readonly AppDBContext _context;
    
    public CategoryViewComponent(AppDBContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }
}