using Xunit;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.AppData;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.WhiteBox
{
    /// <summary>
    /// White Box Tests for ShopController
    /// Testing internal logic, paths, and branches
    /// Test Cases: TC_WB_SHOP_001 - TC_WB_SHOP_010
    /// </summary>
    public class ShopControllerWhiteBoxTests
    {
        #region Helper Methods
        
        private AppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopWBTestDB_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product A", Price = 100, CategoryId = 1, Sales = 50 },
                new Product { Id = 2, Name = "Product B", Price = 200, CategoryId = 1, Sales = 100 },
                new Product { Id = 3, Name = "Product C", Price = 300, CategoryId = 2, Sales = 25 },
                new Product { Id = 4, Name = "Product D", Price = 400, CategoryId = 2, Sales = 75 },
                new Product { Id = 5, Name = "Product E", Price = 500, CategoryId = 3, Sales = 200 }
            );
            context.SaveChanges();
            
            return context;
        }

        private AppDBContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "EmptyShopWBTestDB_" + System.Guid.NewGuid())
                .Options;
            return new AppDBContext(options);
        }

        #endregion

        #region TC_WB_SHOP_001: Index - Statement Coverage

        /// <summary>
        /// TC_WB_SHOP_001: Test Index executes all statements
        /// Statement Coverage: _context.Products.ToList(), return View(products)
        /// </summary>
        [Fact]
        public void Index_ExecutesAllStatements_ReturnsProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Equal(5, products.Count);
        }

        #endregion

        #region TC_WB_SHOP_002: Index - Path Coverage (Empty Database)

        /// <summary>
        /// TC_WB_SHOP_002: Test Index with empty database
        /// Path Coverage: ToList() returns empty collection
        /// </summary>
        [Fact]
        public void Index_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var context = GetEmptyDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Empty(products);
        }

        #endregion

        #region TC_WB_SHOP_003: Index - Data Flow (Products)

        /// <summary>
        /// TC_WB_SHOP_003: Test Index data flow from database to view
        /// Data Flow: products = _context.Products.ToList() -> View(products)
        /// </summary>
        [Fact]
        public void Index_DataFlow_ProductsPassedToView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Verify data integrity
            Assert.Contains(products, p => p.Name == "Product A" && p.Price == 100);
            Assert.Contains(products, p => p.Name == "Product E" && p.Price == 500);
        }

        #endregion

        #region TC_WB_SHOP_004: Index - Product Properties Coverage

        /// <summary>
        /// TC_WB_SHOP_004: Test all product properties are retrieved
        /// Statement Coverage: All properties accessed
        /// </summary>
        [Fact]
        public void Index_RetrievesAllProductProperties()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            var firstProduct = products.First(p => p.Id == 1);
            
            // Assert - All properties
            Assert.Equal(1, firstProduct.Id);
            Assert.Equal("Product A", firstProduct.Name);
            Assert.Equal(100, firstProduct.Price);
            Assert.Equal(1, firstProduct.CategoryId);
            Assert.Equal(50, firstProduct.Sales);
        }

        #endregion

        #region TC_WB_SHOP_005: Index - ViewResult Type Coverage

        /// <summary>
        /// TC_WB_SHOP_005: Test Index returns correct ViewResult type
        /// Statement Coverage: return View(products)
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult_NotOtherActionResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsNotType<JsonResult>(result);
            Assert.IsNotType<RedirectResult>(result);
        }

        #endregion

        #region TC_WB_SHOP_006: Index - Single Product Database

        /// <summary>
        /// TC_WB_SHOP_006: Test Index with single product
        /// Boundary: Minimum products (1)
        /// </summary>
        [Fact]
        public void Index_SingleProduct_ReturnsSingleProduct()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "SingleProductDB_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            context.Products.Add(new Product { Id = 1, Name = "Only Product", Price = 99, CategoryId = 1 });
            context.SaveChanges();
            
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Single(products);
            Assert.Equal("Only Product", products[0].Name);
        }

        #endregion

        #region TC_WB_SHOP_007: Index - Multiple Categories Coverage

        /// <summary>
        /// TC_WB_SHOP_007: Test Index returns products from all categories
        /// Loop Coverage: ToList() iterates all records
        /// </summary>
        [Fact]
        public void Index_ReturnsProducts_FromAllCategories()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert - Products from categories 1, 2, 3
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
            Assert.Contains(1, categoryIds);
            Assert.Contains(2, categoryIds);
            Assert.Contains(3, categoryIds);
        }

        #endregion

        #region TC_WB_SHOP_008: Index - Logger Injection Coverage

        /// <summary>
        /// TC_WB_SHOP_008: Test controller with logger dependency
        /// Statement Coverage: Constructor with ILogger<ShopController>
        /// </summary>
        [Fact]
        public void Index_WithLogger_WorksCorrectly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region TC_WB_SHOP_009: Index - Product Order Coverage

        /// <summary>
        /// TC_WB_SHOP_009: Test Index returns products in database order
        /// Data Flow: Order of products in ToList()
        /// </summary>
        [Fact]
        public void Index_ReturnsProducts_InDatabaseOrder()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert - First product should be ID 1
            Assert.Equal(1, products[0].Id);
        }

        #endregion

        #region TC_WB_SHOP_010: Index - Price Null Handling

        /// <summary>
        /// TC_WB_SHOP_010: Test Index handles nullable price
        /// Condition Coverage: Price can be null (double?)
        /// </summary>
        [Fact]
        public void Index_HandlesNullablePrice_Correctly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "NullPriceDB_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            context.Products.Add(new Product { Id = 1, Name = "No Price Product", Price = null, CategoryId = 1 });
            context.SaveChanges();
            
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Single(products);
            Assert.Null(products[0].Price);
        }

        #endregion
    }
}
