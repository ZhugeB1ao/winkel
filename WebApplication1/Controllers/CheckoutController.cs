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
    private readonly AppDBContext _context;

    public CheckoutController(UserManager<AppUser> userManager, AppDBContext context)
    {
        _userManager = userManager;
        _context = context;
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

    [HttpPost("PlaceOrder")]
    public async Task<IActionResult> PlaceOrder()
    {
        // Check if user is logged in
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "User", new { returnUrl = "/Checkout" });
        }
        
        // Get current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        // Get cart items
        var cart = GetCartItems();
        if (cart == null || cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }
        
        // Create new order
        var order = new Order
        {
            UserId = user.Id,
            Status = "Pending", // or "Processing", "Completed", etc.
            CreatedAt = DateTime.Now,
            OrderProducts = new List<OrderProduct>()
        };
        
        // Add order products from cart
        foreach (var cartItem in cart)
        {
            if (cartItem.Product != null)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = cartItem.Product.Id,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price 
                });
            }
        }
        
        // Save order to database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        // Clear cart
        HttpContext.Session.Remove(CARTKEY);
        
        // Redirect to success page
        return RedirectToAction("OrderSuccess", new { orderId = order.Id });
    }

    [HttpGet("OrderSuccess")]
    public IActionResult OrderSuccess(int? orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }
}