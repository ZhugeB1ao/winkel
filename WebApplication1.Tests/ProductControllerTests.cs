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
    /// Unit tests for ProductController
    /// Based on test cases: TC_WB_PROD_001, TC_WB_PROD_002
    /// </summary>
    public class ProductControllerTests
    {
        private AppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            // Seed categories
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Electronics", ParentId = 0 },
                new Category { Id = 2, Name = "Phones", ParentId = 1 },
                new Category { Id = 3, Name = "Laptops", ParentId = 1 },
                new Category { Id = 4, Name = "Clothing", ParentId = 0 }
            );
            
            // Seed products
            context.Products.AddRange(
                new Product { Id = 1, Name = "iPhone 15", Price = 999, CategoryId = 2 },
                new Product { Id = 2, Name = "Samsung Galaxy", Price = 899, CategoryId = 2 },
                new Product { Id = 3, Name = "MacBook Pro", Price = 2499, CategoryId = 3 },
                new Product { Id = 4, Name = "T-Shirt", Price = 29, CategoryId = 4 }
            );
            context.SaveChanges();
            
            return context;
        }

        [Fact]
        public void Detail_ProductExists_ReturnsViewWithProduct()
        {
            // Arrange - TC_WB_PROD_001 Case 1
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            int existingProductId = 1;
            
            // Act
            var result = controller.Detail(existingProductId);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(existingProductId, model.Id);
            Assert.Equal("iPhone 15", model.Name);
        }

        [Fact]
        public void Detail_ProductNotExists_ReturnsViewWithNull()
        {
            // Arrange - TC_WB_PROD_001 Case 2
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            int nonExistentProductId = 99999;
            
            // Act
            var result = controller.Detail(nonExistentProductId);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void ListPro_CategoryWithChildren_ReturnsProductsFromCategoryAndChildren()
        {
            // Arrange - TC_WB_PROD_002 Case 1
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            int parentCategoryId = 1; // Electronics (has children: Phones, Laptops)
            
            // Act
            var result = controller.ListPro(parentCategoryId);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Product>>(viewResult.Model);
            
            // Should include products from category 1, 2, and 3 (Electronics, Phones, Laptops)
            Assert.Equal(3, model.Count);
            Assert.Contains(model, p => p.Name == "iPhone 15");
            Assert.Contains(model, p => p.Name == "Samsung Galaxy");
            Assert.Contains(model, p => p.Name == "MacBook Pro");
            Assert.DoesNotContain(model, p => p.Name == "T-Shirt");
        }

        [Fact]
        public void ListPro_CategoryWithoutChildren_ReturnsOnlyProductsFromCategory()
        {
            // Arrange - TC_WB_PROD_002 Case 2
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            int leafCategoryId = 4; // Clothing (no children)
            
            // Act
            var result = controller.ListPro(leafCategoryId);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Product>>(viewResult.Model);
            
            // Should only include products from category 4
            Assert.Single(model);
            Assert.Equal("T-Shirt", model[0].Name);
        }

        [Fact]
        public void ListPro_NonExistentCategory_ReturnsEmptyList()
        {
            // Arrange - TC_WB_PROD_002 Case 3
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            int nonExistentCategoryId = 99999;
            
            // Act
            var result = controller.ListPro(nonExistentCategoryId);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Product>>(viewResult.Model);
            Assert.Empty(model);
        }
    }
}
