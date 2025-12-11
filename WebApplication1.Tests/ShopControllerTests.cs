using Xunit;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.AppData;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApplication1.Tests.Controllers
{
    /// <summary>
    /// Unit tests for ShopController
    /// Based on test case: TC_WB_SHOP_001
    /// </summary>
    public class ShopControllerTests
    {
        private AppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            // Seed test data
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Price = 100, CategoryId = 1 },
                new Product { Id = 2, Name = "Product 2", Price = 200, CategoryId = 1 },
                new Product { Id = 3, Name = "Product 3", Price = 300, CategoryId = 2 }
            );
            context.SaveChanges();
            
            return context;
        }

        [Fact]
        public void Index_ReturnsViewResult_WithAllProducts()
        {
            // Arrange - TC_WB_SHOP_001
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Product>>(viewResult.Model);
            
            // Verify all products are returned
            Assert.Equal(3, model.Count);
            Assert.Contains(model, p => p.Name == "Product 1");
            Assert.Contains(model, p => p.Name == "Product 2");
            Assert.Contains(model, p => p.Name == "Product 3");
        }

        [Fact]
        public void Index_WithNoProducts_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "EmptyDatabase_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Product>>(viewResult.Model);
            Assert.Empty(model);
        }
    }
}
