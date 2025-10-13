using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.AppData;

public class AppDBContext : IdentityDbContext<IdentityUser>
{
    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
           
    }
    
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
}
