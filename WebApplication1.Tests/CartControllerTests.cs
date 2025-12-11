using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;
using WebApplication1.AppData;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.Controllers
{
    /// <summary>
    /// Unit tests for CartController
    /// Based on test cases: TC_WB_CART_001, TC_WB_CART_002, TC_WB_CART_003
    /// </summary>
    public class CartControllerTests
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

        private CartController GetControllerWithMockedSession(AppDBContext context)
        {
            var controller = new CartController(context);
            
            // Mock HttpContext and Session
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            var session = new MockHttpSession();
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            
            return controller;
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCartItems()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithMockedSession(context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void AddToCartAjax_ProductNotFound_ReturnsJsonWithSuccessFalse()
        {
            // Arrange - TC_WB_CART_002 Path 1
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithMockedSession(context);
            int nonExistentProductId = 99999;
            
            // Act
            var result = controller.AddToCartAjax(nonExistentProductId);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic value = jsonResult.Value;
            Assert.False(value.success);
            Assert.Equal("Product not found", value.message);
        }

        [Fact]
        public void AddToCartAjax_ProductExists_ReturnsJsonWithSuccessTrue()
        {
            // Arrange - TC_WB_CART_002 Path 2
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithMockedSession(context);
            int existingProductId = 1;
            
            // Act
            var result = controller.AddToCartAjax(existingProductId);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic value = jsonResult.Value;
            Assert.True(value.success);
            Assert.Equal("Product added to cart successfully", value.message);
            Assert.True(value.cartItemCount > 0);
        }

        [Fact]
        public void RemoveItemAjax_ItemExists_ReturnsJsonWithSuccessTrue()
        {
            // Arrange - TC_WB_CART_003 Branch 1
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithMockedSession(context);
            
            // First add a product
            controller.AddToCartAjax(1);
            
            // Act - Remove the product
            var result = controller.RemoveItemAjax(1);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic value = jsonResult.Value;
            Assert.True(value.success);
            Assert.Equal("Item removed from cart", value.message);
        }

        [Fact]
        public void RemoveItemAjax_ItemNotExists_ReturnsJsonWithSuccessFalse()
        {
            // Arrange - TC_WB_CART_003 Branch 2
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithMockedSession(context);
            int nonExistentProductId = 99999;
            
            // Act
            var result = controller.RemoveItemAjax(nonExistentProductId);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic value = jsonResult.Value;
            Assert.False(value.success);
            Assert.Equal("Item not found in cart", value.message);
        }
    }

    /// <summary>
    /// Mock HttpSession for testing
    /// </summary>
    public class MockHttpSession : Microsoft.AspNetCore.Http.ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

        public bool IsAvailable => true;
        public string Id => "test-session-id";
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public System.Threading.Tasks.Task CommitAsync(System.Threading.CancellationToken cancellationToken = default)
            => System.Threading.Tasks.Task.CompletedTask;

        public System.Threading.Tasks.Task LoadAsync(System.Threading.CancellationToken cancellationToken = default)
            => System.Threading.Tasks.Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value)
            => _sessionStorage.TryGetValue(key, out value);
    }
}
