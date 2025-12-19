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
    /// White Box Tests for ProductController
    /// Testing internal logic, paths, and branches
    /// Test Cases: TC_WB_PROD_001 - TC_WB_PROD_010
    /// </summary>
    public class ProductControllerWhiteBoxTests
    {
        #region Helper Methods
        
        private AppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "ProductWBTestDB_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDBContext(options);
            
            // Seed categories with hierarchy
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Electronics", ParentId = 0 },
                new Category { Id = 2, Name = "Phones", ParentId = 1 },
                new Category { Id = 3, Name = "Laptops", ParentId = 1 },
                new Category { Id = 4, Name = "Clothing", ParentId = 0 },
                new Category { Id = 5, Name = "Accessories", ParentId = 1 }
            );
            
            // Seed products
            context.Products.AddRange(
                new Product { Id = 1, Name = "iPhone 15", Price = 999, CategoryId = 2 },
                new Product { Id = 2, Name = "Samsung Galaxy", Price = 899, CategoryId = 2 },
                new Product { Id = 3, Name = "MacBook Pro", Price = 2499, CategoryId = 3 },
                new Product { Id = 4, Name = "Dell XPS", Price = 1999, CategoryId = 3 },
                new Product { Id = 5, Name = "T-Shirt", Price = 29, CategoryId = 4 },
                new Product { Id = 6, Name = "Phone Case", Price = 15, CategoryId = 5 }
            );
            context.SaveChanges();
            
            return context;
        }

        #endregion

        #region TC_WB_PROD_001: Detail - Path Coverage (Product Found)

        /// <summary>
        /// TC_WB_PROD_001: Test Detail when product exists
        /// Path Coverage: Path where product is found
        /// </summary>
        [Fact]
        public void Detail_ProductExists_ReturnsViewWithProduct()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal("iPhone 15", product.Name);
        }

        #endregion

        #region TC_WB_PROD_002: Detail - Path Coverage (Product Not Found)

        /// <summary>
        /// TC_WB_PROD_002: Test Detail when product not exists
        /// Path Coverage: Path where product is null
        /// </summary>
        [Fact]
        public void Detail_ProductNotExists_ReturnsViewWithNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(99999);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region TC_WB_PROD_003: ListPro - Branch Coverage (Category With Children)

        /// <summary>
        /// TC_WB_PROD_003: Test ListPro with parent category
        /// Branch Coverage: childCategoryIds.Add(id) includes parent
        /// </summary>
        [Fact]
        public void ListPro_ParentCategory_IncludesChildrenProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act - Electronics (parent of Phones, Laptops, Accessories)
            var result = controller.ListPro(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            // Should include products from Phones(2), Laptops(2), Accessories(1) = 5
            Assert.Equal(5, products.Count);
        }

        #endregion

        #region TC_WB_PROD_004: ListPro - Branch Coverage (Leaf Category)

        /// <summary>
        /// TC_WB_PROD_004: Test ListPro with leaf category (no children)
        /// Branch Coverage: childCategoryIds only contains the category itself
        /// </summary>
        [Fact]
        public void ListPro_LeafCategory_ReturnsOnlyCategoryProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act - Clothing (no children)
            var result = controller.ListPro(4);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Single(products);
            Assert.Equal("T-Shirt", products[0].Name);
        }

        #endregion

        #region TC_WB_PROD_005: ListPro - Path Coverage (Non-existent Category)

        /// <summary>
        /// TC_WB_PROD_005: Test ListPro with non-existent category
        /// Path Coverage: No products match the criteria
        /// </summary>
        [Fact]
        public void ListPro_NonExistentCategory_ReturnsEmptyList()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.ListPro(99999);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Empty(products);
        }

        #endregion

        #region TC_WB_PROD_006: ListPro - Data Flow (ChildCategoryIds)

        /// <summary>
        /// TC_WB_PROD_006: Test ListPro childCategoryIds data flow
        /// Data Flow: childCategoryIds is populated from Categories table
        /// </summary>
        [Fact]
        public void ListPro_ChildCategoryIds_CorrectlyPopulated()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act - Phones category (leaf)
            var result = controller.ListPro(2);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Equal(2, products.Count); // Only phones
            Assert.All(products, p => Assert.Equal(2, p.CategoryId));
        }

        #endregion

        #region TC_WB_PROD_007: Detail - Statement Coverage (All Statements)

        /// <summary>
        /// TC_WB_PROD_007: Test Detail executes all statements
        /// Statement Coverage: new Product(), _context.Products.Find(id), View(product)
        /// </summary>
        [Fact]
        public void Detail_ExecutesAllStatements_WhenProductExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Detail(3);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal(3, product.Id);
            Assert.Equal("MacBook Pro", product.Name);
            Assert.Equal(2499, product.Price);
        }

        #endregion

        #region TC_WB_PROD_008: ListPro - Loop Coverage (Multiple Children)

        /// <summary>
        /// TC_WB_PROD_008: Test ListPro iterates through all child categories
        /// Loop Coverage: Where(c => c.ParentId == id) iterates correctly
        /// </summary>
        [Fact]
        public void ListPro_LoopIteratesAllChildren_Correctly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act - Electronics has 3 children: Phones, Laptops, Accessories
            var result = controller.ListPro(1);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // Verify products from all child categories are included
            Assert.Contains(products, p => p.CategoryId == 2); // Phones
            Assert.Contains(products, p => p.CategoryId == 3); // Laptops
            Assert.Contains(products, p => p.CategoryId == 5); // Accessories
        }

        #endregion

        #region TC_WB_PROD_009: Index - Statement Coverage

        /// <summary>
        /// TC_WB_PROD_009: Test Index returns View
        /// Statement Coverage: return View()
        /// </summary>
        [Fact]
        public void Index_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act
            var result = controller.Index();
            
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region TC_WB_PROD_010: ListPro - Condition Coverage

        /// <summary>
        /// TC_WB_PROD_010: Test ListPro condition Where(p => childCategoryIds.Contains(p.CategoryId))
        /// Condition Coverage: Tests the Contains condition
        /// </summary>
        [Fact]
        public void ListPro_ContainsCondition_FiltersCorrectly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProductController(context);
            
            // Act - Laptops category
            var result = controller.ListPro(3);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var products = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            
            // All products should be from Laptops category
            Assert.Equal(2, products.Count);
            Assert.All(products, p => Assert.Equal(3, p.CategoryId));
            Assert.Contains(products, p => p.Name == "MacBook Pro");
            Assert.Contains(products, p => p.Name == "Dell XPS");
        }

        #endregion
    }
}
