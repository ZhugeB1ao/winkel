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
    /// Black Box Tests for ProductController
    /// Techniques: Equivalence Partitioning, Boundary Value Analysis
    /// Test Cases: TC_BB_PROD_001 - TC_BB_PROD_010
    /// </summary>
    public class ProductControllerBlackBoxTests
    {
        #region Helper Methods
        
        private AppDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "ProductBBTestDB_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Electronics", ParentId = 0 },
                new Category { Id = 2, Name = "Phones", ParentId = 1 },
                new Category { Id = 3, Name = "Clothing", ParentId = 0 }
            );
            
            context.Products.AddRange(
                new Product { Id = 1, Name = "iPhone 15", Price = 999, CategoryId = 2 },
                new Product { Id = 2, Name = "Samsung Galaxy", Price = 899, CategoryId = 2 },
                new Product { Id = 3, Name = "Pixel 8", Price = 699, CategoryId = 2 },
                new Product { Id = 4, Name = "T-Shirt", Price = 29, CategoryId = 3 },
                new Product { Id = 5, Name = "Free Item", Price = 0, CategoryId = 3 }
            );
            context.SaveChanges();
            
            return context;
        }

        #endregion

        #region TC_BB_PROD_001: EP - Valid Product ID

        /// <summary>
        /// TC_BB_PROD_001: Test with valid product ID
        /// Equivalence Partitioning: Valid ID class
        /// </summary>
        [Fact]
        public void Detail_ValidProductId_ReturnsProduct()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal("iPhone 15", product.Name);
        }

        #endregion

        #region TC_BB_PROD_002: EP - Invalid Product ID (Negative)

        /// <summary>
        /// TC_BB_PROD_002: Test with negative product ID
        /// Equivalence Partitioning: Invalid ID class (negative)
        /// </summary>
        [Fact]
        public void Detail_NegativeProductId_ReturnsNull()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(-1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region TC_BB_PROD_003: BVA - Product ID = 0 (Boundary)

        /// <summary>
        /// TC_BB_PROD_003: Test with product ID = 0
        /// Boundary Value Analysis: Lower boundary
        /// </summary>
        [Fact]
        public void Detail_ZeroProductId_ReturnsNull()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(0);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region TC_BB_PROD_004: BVA - Product ID = Max (Boundary)

        /// <summary>
        /// TC_BB_PROD_004: Test with maximum integer product ID
        /// Boundary Value Analysis: Upper boundary
        /// </summary>
        [Fact]
        public void Detail_MaxIntProductId_ReturnsNull()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(int.MaxValue);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region TC_BB_PROD_005: EP - Category With Products

        /// <summary>
        /// TC_BB_PROD_005: Test ListPro with category containing products
        /// Equivalence Partitioning: Valid category with products
        /// </summary>
        [Fact]
        public void ListPro_CategoryWithProducts_ReturnsProducts()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.ListPro(2); // Phones category
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Equal(3, products.Count);
        }

        #endregion

        #region TC_BB_PROD_006: EP - Category Without Products

        /// <summary>
        /// TC_BB_PROD_006: Test ListPro with empty category
        /// Equivalence Partitioning: Valid category without products
        /// </summary>
        [Fact]
        public void ListPro_EmptyCategory_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "EmptyCatDB_" + System.Guid.NewGuid())
                .Options;
            var context = new AppDBContext(options);
            context.Categories.Add(new Category { Id = 1, Name = "Empty", ParentId = 0 });
            context.SaveChanges();
            
            var controller = new ProductController(context);
            
            // Act
            var result = controller.ListPro(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Empty(products);
        }

        #endregion

        #region TC_BB_PROD_007: EP - Non-existent Category

        /// <summary>
        /// TC_BB_PROD_007: Test ListPro with non-existent category
        /// Equivalence Partitioning: Invalid category
        /// </summary>
        [Fact]
        public void ListPro_NonExistentCategory_ReturnsEmptyList()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.ListPro(99999);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Empty(products);
        }

        #endregion

        #region TC_BB_PROD_008: EP - Parent Category (Includes Children)

        /// <summary>
        /// TC_BB_PROD_008: Test ListPro with parent category
        /// Equivalence Partitioning: Parent category class
        /// </summary>
        [Fact]
        public void ListPro_ParentCategory_IncludesChildrenProducts()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act - Electronics is parent of Phones
            var result = controller.ListPro(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Equal(3, products.Count); // All phones
        }

        #endregion

        #region TC_BB_PROD_009: BVA - First Product ID (ID = 1)

        /// <summary>
        /// TC_BB_PROD_009: Test with first valid product ID
        /// Boundary Value Analysis: First valid ID
        /// </summary>
        [Fact]
        public void Detail_FirstProductId_ReturnsProduct()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal(1, product.Id);
        }

        #endregion

        #region TC_BB_PROD_010: BVA - Last Product ID

        /// <summary>
        /// TC_BB_PROD_010: Test with last valid product ID
        /// Boundary Value Analysis: Last valid ID
        /// </summary>
        [Fact]
        public void Detail_LastProductId_ReturnsProduct()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(5);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal(5, product.Id);
            Assert.Equal("Free Item", product.Name);
        }

        #endregion
    }
}
