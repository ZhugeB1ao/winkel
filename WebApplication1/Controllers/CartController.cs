using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SQLitePCL;
using WebApplication1.AppData;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Route("Cart")]
public class CartController : Controller
{
    private readonly AppDBContext _context;
    
    public CartController(AppDBContext context)
    {
        _context = context;
    }
    
    // Key save json string of cart
    public const string CARTKEY = "cart";
    
    // Get cart from session
    List<CartItem> GetCartItems()
    {
        var session = HttpContext.Session;
        string jsoncart = session.GetString(CARTKEY);
        if (jsoncart != null) return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);

        return new List<CartItem>();
    }
    
    // Remove cart from session
    void ClearCart()
    {
        var session = HttpContext.Session;
        session.Remove(CARTKEY);
    }
    
    // Save cart (list cart) to session
    void SaveCart(List<CartItem> cart)
    {
        var session = HttpContext.Session;
        string jsoncart = JsonConvert.SerializeObject(cart);
        session.SetString(CARTKEY, jsoncart);
    }
    
    
    [HttpGet("")]
    // [HttpGet("Index")]
    public IActionResult Index()
    {
        var cart = GetCartItems();
        ViewBag.SubTotal = cart.Sum(item => (item.Product.Price ?? 0) * item.Quantity);
        ViewBag.Delivery = 0;
        ViewBag.Discount = 0;
        ViewBag.Total = ViewBag.SubTotal + ViewBag.Delivery - ViewBag.Discount;
        return View(cart);
    }
    
    [HttpPost("AddItemAjax")]
    public IActionResult AddItemAjax([FromBody] int productId)
    {
        // Find product in dtb
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if(product == null) 
            return Json(new { success = false, message = "Product not found" });
        
        // Get cart from session
        var cart = GetCartItems();
        
        // Check if the product already exists in cart
        var cartItem = cart.FirstOrDefault(p => p.Product.Id == productId);

        if (cartItem != null)
        {
            cartItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem()
            {
                Quantity = 1,
                Product = product
            });
        }
        
        SaveCart(cart);
        
        // recalculate totals
        double subtotal = cart.Sum(c => (c.Product.Price ?? 0) * c.Quantity);
        double delivery = 0;
        double discount = 0;
        double total = subtotal + delivery - discount;
        
        // Return JSON response
        return Json(new { 
            success = true, 
            message = "Product added to cart successfully",
            cartItemCount = cart.Sum(c => c.Quantity),
            subtotal = subtotal,
            total = total
        });
    }
    
    [HttpPost("RemoveItemAjax")]
    public IActionResult RemoveItemAjax([FromBody] int productId)
    {
        // Get cart from session
        var cart = GetCartItems();
        
        // Find and remove item from cart
        var cartItem = cart.FirstOrDefault(p => p.Product.Id == productId);
        
        if (cartItem != null)
        {
            cart.Remove(cartItem);
            SaveCart(cart);
            
            // recalculate totals
            double subtotal = cart.Sum(c => (c.Product.Price ?? 0) * c.Quantity);
            double delivery = 0;
            double discount = 0;
            double total = subtotal + delivery - discount;
            
            return Json(new { 
                success = true, 
                message = "Item removed from cart",
                cartItemCount = cart.Sum(c => c.Quantity),
                remainingItems = cart.Count,
                subtotal = subtotal,
                total = total
            });
        }
        
        return Json(new { 
            success = false, 
            message = "Item not found in cart" 
        });
    }

    [HttpPost("UpdateItemAjax")]
    public IActionResult UpdateItemAjax([FromBody] UpdateQuantityRequest request)
    {
        var cart = GetCartItems();
        var cartItem = cart.FirstOrDefault(p => p.Product.Id == request.ProductId);
        
        if (cartItem != null)
        {
            cartItem.Quantity = request.Quantity;
            SaveCart(cart);
            
            // recalculate totals
            double subtotal = cart.Sum(c => (c.Product.Price ?? 0) * c.Quantity);
            double delivery = 0;
            double discount = 0;
            double total = subtotal + delivery - discount;
            
            return Json(new { 
                success = true,
                itemTotal = (cartItem.Product.Price ?? 0) * cartItem.Quantity,
                subtotal = subtotal,
                total = total,
                cartItemCount = cart.Sum(c => c.Quantity)
            });
        }
        
        return Json(new { success = false });
    }
}