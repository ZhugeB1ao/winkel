using Xunit;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.AppData;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApplication1.Tests.WhiteBox
{
    /// <summary>
    /// White Box Tests for CartController
    /// Testing internal logic, paths, and branches
    /// Test Cases: TC_WB_CART_001 - TC_WB_CART_010
    /// </summary>
    public class CartControllerWhiteBoxTests
    {
        #region Helper Methods
        
        private AppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "CartWBTestDB_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product 1", Price = 100, CategoryId = 1 },
                new Product { Id = 2, Name = "Product 2", Price = 200, CategoryId = 1 },
                new Product { Id = 3, Name = "Product 3", Price = 300, CategoryId = 2 },
                new Product { Id = 4, Name = "Product 4", Price = 0, CategoryId = 2 },
                new Product { Id = 5, Name = "Product 5", Price = 50.5, CategoryId = 1 }
            );
            context.SaveChanges();
            
            return context;
        }

        private CartController GetControllerWithSession(AppDBContext context)
        {
            var controller = new CartController(context);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            var session = new TestMockSession();
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            return controller;
        }
        
        private T GetPropertyValue<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return (T)property?.GetValue(obj)!;
        }

        #endregion

        #region TC_WB_CART_001: Index - Statement Coverage

        /// <summary>
        /// TC_WB_CART_001: Test Index returns ViewResult with cart model
        /// Statement Coverage: All statements in Index method
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult_WithCartModel()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        #endregion

        #region TC_WB_CART_002: AddItemAjax - Path Coverage (Product Not Found)

        /// <summary>
        /// TC_WB_CART_002: Test AddItemAjax when product not found
        /// Path Coverage: Path where product is null
        /// </summary>
        [Fact]
        public void AddItemAjax_ProductNotFound_ReturnsFailure()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            
            // Act
            var result = controller.AddItemAjax(99999);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            Assert.False(success);
        }

        #endregion

        #region TC_WB_CART_003: AddItemAjax - Path Coverage (New Product)

        /// <summary>
        /// TC_WB_CART_003: Test AddItemAjax when adding new product to cart
        /// Path Coverage: Path where cartItem is null (new item)
        /// </summary>
        [Fact]
        public void AddItemAjax_NewProduct_AddsToCart()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            
            // Act
            var result = controller.AddItemAjax(1);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            var cartItemCount = GetPropertyValue<int>(jsonResult.Value, "cartItemCount");
            Assert.True(success);
            Assert.Equal(1, cartItemCount);
        }

        #endregion

        #region TC_WB_CART_004: AddItemAjax - Branch Coverage (Existing Product)

        /// <summary>
        /// TC_WB_CART_004: Test AddItemAjax when product already in cart
        /// Branch Coverage: Branch where cartItem exists (quantity++)
        /// </summary>
        [Fact]
        public void AddItemAjax_ExistingProduct_IncrementsQuantity()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            controller.AddItemAjax(1); // Add first time
            
            // Act
            var result = controller.AddItemAjax(1); // Add second time
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var cartItemCount = GetPropertyValue<int>(jsonResult.Value, "cartItemCount");
            Assert.Equal(2, cartItemCount);
        }

        #endregion

        #region TC_WB_CART_005: RemoveItemAjax - Path Coverage (Item Found)

        /// <summary>
        /// TC_WB_CART_005: Test RemoveItemAjax when item exists in cart
        /// Path Coverage: Path where cartItem is found
        /// </summary>
        [Fact]
        public void RemoveItemAjax_ItemExists_RemovesFromCart()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            controller.AddItemAjax(1);
            
            // Act
            var result = controller.RemoveItemAjax(1);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            var remainingItems = GetPropertyValue<int>(jsonResult.Value, "remainingItems");
            Assert.True(success);
            Assert.Equal(0, remainingItems);
        }

        #endregion

        #region TC_WB_CART_006: RemoveItemAjax - Path Coverage (Item Not Found)

        /// <summary>
        /// TC_WB_CART_006: Test RemoveItemAjax when item not in cart
        /// Path Coverage: Path where cartItem is not found
        /// </summary>
        [Fact]
        public void RemoveItemAjax_ItemNotExists_ReturnsFailure()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            
            // Act
            var result = controller.RemoveItemAjax(99999);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            Assert.False(success);
        }

        #endregion

        #region TC_WB_CART_007: Index - ViewBag Calculation Coverage

        /// <summary>
        /// TC_WB_CART_007: Test Index calculates ViewBag values correctly
        /// Statement Coverage: ViewBag.SubTotal, Delivery, Discount, Total
        /// </summary>
        [Fact]
        public void Index_CalculatesViewBagValues_Correctly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            controller.AddItemAjax(1); // Price: 100
            controller.AddItemAjax(2); // Price: 200
            
            // Act
            var result = controller.Index() as ViewResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(300.0, controller.ViewBag.SubTotal);
            Assert.Equal(0, controller.ViewBag.Delivery);
            Assert.Equal(0, controller.ViewBag.Discount);
            Assert.Equal(300.0, controller.ViewBag.Total);
        }

        #endregion

        #region TC_WB_CART_008: AddItemAjax - Subtotal Calculation

        /// <summary>
        /// TC_WB_CART_008: Test AddItemAjax calculates subtotal correctly
        /// Data Flow: subtotal = cart.Sum(c => (c.Product.Price ?? 0) * c.Quantity)
        /// </summary>
        [Fact]
        public void AddItemAjax_CalculatesSubtotal_Correctly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            
            // Act
            controller.AddItemAjax(1); // 100
            var result = controller.AddItemAjax(2); // 200
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var subtotal = GetPropertyValue<double>(jsonResult.Value!, "subtotal");
            Assert.Equal(300.0, subtotal);
        }

        #endregion

        #region TC_WB_CART_009: UpdateItemAjax - Path Coverage (Item Found)

        /// <summary>
        /// TC_WB_CART_009: Test UpdateItemAjax when item exists
        /// Path Coverage: Path where cartItem is found
        /// </summary>
        [Fact]
        public void UpdateItemAjax_ItemExists_UpdatesQuantity()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            controller.AddItemAjax(1);
            var request = new UpdateQuantityRequest { ProductId = 1, Quantity = 5 };
            
            // Act
            var result = controller.UpdateItemAjax(request);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            var cartItemCount = GetPropertyValue<int>(jsonResult.Value, "cartItemCount");
            Assert.True(success);
            Assert.Equal(5, cartItemCount);
        }

        #endregion

        #region TC_WB_CART_010: UpdateItemAjax - Path Coverage (Item Not Found)

        /// <summary>
        /// TC_WB_CART_010: Test UpdateItemAjax when item not in cart
        /// Path Coverage: Path where cartItem is null
        /// </summary>
        [Fact]
        public void UpdateItemAjax_ItemNotExists_ReturnsFailure()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithSession(context);
            var request = new UpdateQuantityRequest { ProductId = 99999, Quantity = 5 };
            
            // Act
            var result = controller.UpdateItemAjax(request);
            
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var success = GetPropertyValue<bool>(jsonResult.Value, "success");
            Assert.False(success);
        }

        #endregion
    }

    /// <summary>
    /// Mock Session for testing
    /// </summary>
    public class TestMockSession : Microsoft.AspNetCore.Http.ISession
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
            => _sessionStorage.TryGetValue(key, out value!);
    }
}
