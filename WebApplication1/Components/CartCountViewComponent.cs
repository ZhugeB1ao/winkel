using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Controllers;
using WebApplication1.Models;

namespace WebApplication1.Components;

/// <summary>
/// View component that displays the total count of items in the shopping cart.
/// This component is typically used in the layout header to show a real-time cart item count.
/// </summary>
public class CartCountViewComponent : ViewComponent
{
    /// <summary>
    /// Invokes the view component to calculate and return the total cart item count.
    /// </summary>
    /// <returns>A string representation of the total quantity of items in the cart.</returns>
    public IViewComponentResult Invoke()
    {
        // Get the current user session
        var session = HttpContext.Session;
        
        // Retrieve the cart data from session storage using the cart key
        string jsoncart = session.GetString(CartController.CARTKEY);
        
        // Initialize cart count to zero
        int cartCount = 0;
        
        // Check if cart data exists in session
        if (jsoncart != null)
        {
            // Deserialize the JSON string to a list of CartItem objects
            var cart = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            
            // Calculate total quantity by summing all item quantities
            // Uses null-coalescing operator to return 0 if cart is null
            cartCount = cart?.Sum(c => c.Quantity) ?? 0;
        }
        
        // Return the cart count as a string content result (return text)
        // Content is a helper method in BaseController/ViewComponent
        return Content(cartCount.ToString());
    }
}
