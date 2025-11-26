namespace WebApplication1.Models;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    
    public int? ParentId { get; set; } = 0;
    public Category? Parent { get; set; }
    
    public ICollection<Category>? Children { get; set; }
    
    public ICollection<Product>? Products { get; set; }	
}