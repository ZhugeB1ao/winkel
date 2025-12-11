# 🚀 Quick Start - Running Tests

## Cách nhanh nhất để chạy tests với giao diện đẹp:

### macOS/Linux:
```bash
cd WebApplication1.Tests
./run-tests.sh
```

### Windows PowerShell:
```powershell
cd WebApplication1.Tests
.\run-tests.ps1
```

## Output mẫu:

```
╔════════════════════════════════════════════════════════════╗
║          WebApplication1 - Test Suite Runner              ║
╚════════════════════════════════════════════════════════════╝

→ Running tests...

┌─────────────────────────────────────────────────────────┐
│                    TEST SUMMARY                         │
├─────────────────────────────────────────────────────────┤
│ ✓ Passed:  17                                           │
│ ✗ Failed:  4                                            │
│ ★ Total:   21                                           │
│ ⏱  Duration: 919 ms                                      │
├─────────────────────────────────────────────────────────┤
│ Pass Rate:  80% ⚡                                      │
└─────────────────────────────────────────────────────────┘

Test Results by Class:

✓ ProductControllerTests - 5/5 PASSED (100%)
✓ ViewModelValidationTests - 10/10 PASSED (100%)
✓ ShopControllerTests - 2/2 PASSED (100%)
✗ CartControllerTests - 1/5 PASSED (20%) [Known Issue: Session Mocking]
```

## Các lệnh khác:

```bash
# Chạy tests với dotnet CLI (output mặc định)
dotnet test

# Chạy với output chi tiết
dotnet test --verbosity normal

# Chạy chỉ tests passing
dotnet test --filter "FullyQualifiedName!~CartControllerTests.AddToCartAjax&FullyQualifiedName!~CartControllerTests.RemoveItemAjax"

# Chạy một test class cụ thể
dotnet test --filter "FullyQualifiedName~ProductControllerTests"
```

## Tài liệu:

- 📖 **README.md** - Hướng dẫn chi tiết
- 📊 **TEST_RESULTS.md** - Kết quả và phân tích
- 📝 **TestCases.txt** - 110+ test cases (black box + white box)
