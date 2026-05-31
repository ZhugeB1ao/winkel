using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using winkel.AppData;

namespace winkel.Components;

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