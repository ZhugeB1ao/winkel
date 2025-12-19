using Xunit;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.AppData;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.BlackBox
{
    /// <summary>
    /// Black Box Tests for ShopController
    /// Techniques: Equivalence Partitioning, Boundary Value Analysis
    /// Test Cases: TC_BB_SHOP_001 - TC_BB_SHOP_010
    /// </summary>
    public class ShopControllerBlackBoxTests
    {
        #region Helper Methods
        
        private AppDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopBBTestDB_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            context.Products.AddRange(
                new Product { Id = 1, Name = "Product A", Price = 100, CategoryId = 1 },
                new Product { Id = 2, Name = "Product B", Price = 200, CategoryId = 1 },
                new Product { Id = 3, Name = "Product C", Price = 0, CategoryId = 2 },
                new Product { Id = 4, Name = "Product D", Price = 999.99, CategoryId = 2 },
                new Product { Id = 5, Name = "Product E", Price = 50, CategoryId = 3 }
            );
            context.SaveChanges();
            
            return context;
        }

        private AppDBContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "EmptyShopBBTestDB_" + System.Guid.NewGuid())
                .Options;
            return new AppDBContext(options);
        }

        #endregion

        #region TC_BB_SHOP_001: EP - Database With Products

        /// <summary>
        /// TC_BB_SHOP_001: Test Index with products in database
        /// Equivalence Partitioning: Non-empty database class
        /// </summary>
        [Fact]
        public void Index_DatabaseWithProducts_ReturnsAllProducts()
        {
            // Arrange
            var context = GetDbContext();
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

        #region TC_BB_SHOP_002: EP - Empty Database

        /// <summary>
        /// TC_BB_SHOP_002: Test Index with empty database
        /// Equivalence Partitioning: Empty database class
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

        #region TC_BB_SHOP_003: BVA - Single Product

        /// <summary>
        /// TC_BB_SHOP_003: Test Index with single product
        /// Boundary Value Analysis: Minimum products (1)
        /// </summary>
        [Fact]
        public void Index_SingleProduct_ReturnsSingleProduct()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "SingleDB_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            context.Products.Add(new Product { Id = 1, Name = "Only One", Price = 50, CategoryId = 1 });
            context.SaveChanges();
            
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Single(products);
        }

        #endregion

        #region TC_BB_SHOP_004: EP - Products With Zero Price

        /// <summary>
        /// TC_BB_SHOP_004: Test Index includes products with zero price
        /// Equivalence Partitioning: Free products class
        /// </summary>
        [Fact]
        public void Index_ProductsWithZeroPrice_IncludesAll()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert - Should include product with price = 0
            Assert.Contains(products, p => p.Price == 0);
        }

        #endregion

        #region TC_BB_SHOP_005: EP - Products From Multiple Categories

        /// <summary>
        /// TC_BB_SHOP_005: Test Index returns products from all categories
        /// Equivalence Partitioning: Multiple categories class
        /// </summary>
        [Fact]
        public void Index_MultipleCategories_ReturnsFromAll()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert - Products from categories 1, 2, 3
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
            Assert.Equal(3, categoryIds.Count);
        }

        #endregion

        #region TC_BB_SHOP_006: BVA - Product Price Boundary (0)

        /// <summary>
        /// TC_BB_SHOP_006: Test product with minimum valid price (0)
        /// Boundary Value Analysis: Price = 0
        /// </summary>
        [Fact]
        public void Index_ProductPriceZero_ReturnedCorrectly()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            var freeProduct = products.First(p => p.Price == 0);
            
            // Assert
            Assert.Equal("Product C", freeProduct.Name);
        }

        #endregion

        #region TC_BB_SHOP_007: EP - Products With Decimal Prices

        /// <summary>
        /// TC_BB_SHOP_007: Test products with decimal prices
        /// Equivalence Partitioning: Decimal price class
        /// </summary>
        [Fact]
        public void Index_DecimalPrices_ReturnedCorrectly()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert - Check decimal price
            Assert.Contains(products, p => p.Price == 999.99);
        }

        #endregion

        #region TC_BB_SHOP_008: EP - Products With Null Price

        /// <summary>
        /// TC_BB_SHOP_008: Test products with null price
        /// Equivalence Partitioning: Null price class
        /// </summary>
        [Fact]
        public void Index_NullPrice_ReturnedCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "NullPriceBB_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            context.Products.Add(new Product { Id = 1, Name = "No Price", Price = null, CategoryId = 1 });
            context.SaveChanges();
            
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Assert
            Assert.Single(products);
            Assert.Null(products[0].Price);
        }

        #endregion

        #region TC_BB_SHOP_009: EP - Valid Output Type

        /// <summary>
        /// TC_BB_SHOP_009: Test Index returns correct output type
        /// Equivalence Partitioning: Output type validation
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult_NotOtherTypes()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsNotType<JsonResult>(result);
            Assert.IsNotType<RedirectResult>(result);
            Assert.IsNotType<NotFoundResult>(result);
        }

        #endregion

        #region TC_BB_SHOP_010: EP - Model Type Validation

        /// <summary>
        /// TC_BB_SHOP_010: Test Index returns correct model type
        /// Equivalence Partitioning: Model type validation
        /// </summary>
        [Fact]
        public void Index_ModelIsListOfProducts()
        {
            // Arrange
            var context = GetDbContext();
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ShopController>();
            var controller = new ShopController(logger, context);
            
            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            
            // Assert
            Assert.IsType<List<Product>>(viewResult.Model);
            Assert.IsNotType<Product>(viewResult.Model);
            Assert.IsNotType<List<Category>>(viewResult.Model);
        }

        #endregion
    }
}
