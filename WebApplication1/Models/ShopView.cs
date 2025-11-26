namespace WebApplication1.Models;

public class ShopView
{
    public List<Category> Categories { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public Category? CurrentCategory { get; set; }
    public Product? CurrentProduct { get; set; }
}