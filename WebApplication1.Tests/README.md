# WebApplication1.Tests

Unit Testing Project cho WebApplication1 E-Commerce Application

## 📋 Tổng quan

Project này chứa unit tests cho WebApplication1 sử dụng:
- **xUnit** - Testing framework
- **Moq** - Mocking framework  
- **EntityFrameworkCore.InMemory** - In-memory database cho testing
- **Microsoft.AspNetCore.Mvc.Testing** - Testing utilities cho ASP.NET Core

## 📊 Test Coverage

### Tổng số: 22 Unit Tests

#### 1. CartControllerTests.cs (5 tests)
- ✅ `Index_ReturnsViewResult_WithCartItems`
- ✅ `AddToCartAjax_ProductNotFound_ReturnsJsonWithSuccessFalse`
- ✅ `AddToCartAjax_ProductExists_ReturnsJsonWithSuccessTrue`
- ✅ `RemoveItemAjax_ItemExists_ReturnsJsonWithSuccessTrue`
- ✅ `RemoveItemAjax_ItemNotExists_ReturnsJsonWithSuccessFalse`

**Coverage**: TC_WB_CART_001, TC_WB_CART_002, TC_WB_CART_003

#### 2. ProductControllerTests.cs (5 tests)
- ✅ `Detail_ProductExists_ReturnsViewWithProduct`
- ✅ `Detail_ProductNotExists_ReturnsViewWithNull`
- ✅ `ListPro_CategoryWithChildren_ReturnsProductsFromCategoryAndChildren`
- ✅ `ListPro_CategoryWithoutChildren_ReturnsOnlyProductsFromCategory`
- ✅ `ListPro_NonExistentCategory_ReturnsEmptyList`

**Coverage**: TC_WB_PROD_001, TC_WB_PROD_002

#### 3. ViewModelValidationTests.cs (10 tests)

**RegisterViewModel (6 tests)**:
- ✅ `RegisterViewModel_ValidData_PassesValidation`
- ✅ `RegisterViewModel_InvalidEmail_FailsValidation`
- ✅ `RegisterViewModel_PasswordTooShort_FailsValidation`
- ✅ `RegisterViewModel_PasswordMismatch_FailsValidation`
- ✅ `RegisterViewModel_MissingRequiredField_FailsValidation`
- ✅ `RegisterViewModel_InvalidPhone_FailsValidation`

**LoginViewModel (4 tests)**:
- ✅ `LoginViewModel_ValidData_PassesValidation`
- ✅ `LoginViewModel_MissingEmail_FailsValidation`
- ✅ `LoginViewModel_PasswordTooShort_FailsValidation`

**Coverage**: TC_WB_MODEL_001, TC_WB_MODEL_002

#### 4. ShopControllerTests.cs (2 tests)
- ✅ `Index_ReturnsViewResult_WithAllProducts`
- ✅ `Index_WithNoProducts_ReturnsEmptyList`

**Coverage**: TC_WB_SHOP_001

## 🚀 Chạy Tests

### Cách 1: Sử dụng Test Runner Script (Recommended) 🎨

**macOS/Linux:**
```bash
./run-tests.sh
```

**Windows PowerShell:**
```powershell
.\run-tests.ps1
```

**Features:**
- ✨ Giao diện đẹp với màu sắc
- 📊 Tóm tắt kết quả rõ ràng
- 🎯 Hiển thị pass rate
- ⚡ Breakdown theo test class
- 🐛 Highlight known issues

### Cách 2: Sử dụng dotnet CLI

#### Chạy tất cả tests
```bash
dotnet test
```

#### Chạy tests với output chi tiết
```bash
dotnet test --verbosity normal
```

### Chạy tests với code coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Chạy một test cụ thể
```bash
dotnet test --filter "FullyQualifiedName~CartControllerTests"
```

### Chạy tests theo category
```bash
dotnet test --filter "Category=Controllers"
```

## 📁 Cấu trúc Project

```
WebApplication1.Tests/
├── CartControllerTests.cs          # Tests cho CartController
├── ProductControllerTests.cs       # Tests cho ProductController
├── ShopControllerTests.cs          # Tests cho ShopController
├── ViewModelValidationTests.cs     # Tests cho Model Validation
├── WebApplication1.Tests.csproj    # Project file
└── README.md                       # File này
```

## 🔧 Dependencies

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

## 📝 Viết Tests Mới

### Template cho Controller Test

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new YourController(context);
    
    // Act
    var result = controller.YourMethod();
    
    // Assert
    Assert.NotNull(result);
}
```

### Template cho Validation Test

```csharp
[Fact]
public void Model_InvalidData_FailsValidation()
{
    // Arrange
    var model = new YourModel { /* invalid data */ };
    
    // Act
    var results = ValidateModel(model);
    
    // Assert
    Assert.NotEmpty(results);
}
```

## 🎯 Best Practices

1. **Naming Convention**: `MethodName_Scenario_ExpectedBehavior`
2. **AAA Pattern**: Arrange, Act, Assert
3. **One Assert Per Test**: Mỗi test nên test một behavior cụ thể
4. **Independent Tests**: Tests không phụ thuộc vào nhau
5. **Use InMemory Database**: Mỗi test dùng database riêng (Guid.NewGuid())
6. **Mock External Dependencies**: Sử dụng Moq để mock dependencies

## 📚 Tài liệu tham khảo

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [ASP.NET Core Testing](https://learn.microsoft.com/en-us/aspnet/core/test/)
- [EF Core In-Memory Database](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/)

## 🐛 Troubleshooting

### Lỗi: "Database already exists"
**Giải pháp**: Sử dụng `Guid.NewGuid()` trong database name để tạo unique database cho mỗi test

### Lỗi: "Null reference exception"
**Giải pháp**: Kiểm tra mock setup và ensure dependencies được inject đúng

### Tests chạy chậm
**Giải pháp**: Sử dụng InMemory database thay vì real database, và tránh I/O operations

## 📈 Roadmap

- [ ] Thêm Integration Tests
- [ ] Thêm tests cho UserController (với Identity mocking)
- [ ] Implement Code Coverage reporting
- [ ] Thêm Performance Tests
- [ ] Setup CI/CD pipeline

## 👨‍💻 Contributors

- Test Cases được thiết kế dựa trên TestCases.txt
- Implemented với xUnit và Moq

---

**Last Updated**: 2025-12-11
