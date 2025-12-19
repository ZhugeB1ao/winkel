using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.AppData;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Route("Checkout")]
public class CheckoutController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    
    public CheckoutController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    
    public const string CARTKEY = "cart";
    
    // Get cart from session
    List<CartItem> GetCartItems()
    {
        var session = HttpContext.Session;
        string jsoncart = session.GetString(CARTKEY);
        if (jsoncart != null) return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);

        return new List<CartItem>();
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // Check if user is logged in
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "User", new { returnUrl = "/Checkout" });
        }
        
        // Get current user info
        var user = await _userManager.GetUserAsync(User);
        
        // Split FullName into first and last name
        var nameParts = (user?.FullName ?? "").Split(' ', 2);
        ViewBag.FirstName = nameParts.Length > 0 ? nameParts[0] : "";
        ViewBag.LastName = nameParts.Length > 1 ? nameParts[1] : "";
        ViewBag.Address = user?.Address ?? "";
        ViewBag.Phone = user?.PhoneNumber ?? "";
        ViewBag.Email = user?.Email ?? "";
        
        var cart = GetCartItems();
        ViewBag.SubTotal = cart.Sum(item => (item.Product.Price ?? 0) * item.Quantity);
        ViewBag.Delivery = 0;
        ViewBag.Discount = 0;
        ViewBag.Total = ViewBag.SubTotal + ViewBag.Delivery - ViewBag.Discount;
        return View(cart);
    }
}