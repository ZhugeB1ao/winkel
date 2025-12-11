using Xunit;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.Models
{
    /// <summary>
    /// Unit tests for Model Validations
    /// Based on test cases: TC_WB_MODEL_001, TC_WB_MODEL_002
    /// </summary>
    public class ViewModelValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #region RegisterViewModel Tests

        [Fact]
        public void RegisterViewModel_ValidData_PassesValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Nguyen Van A",
                Phone = "0912345678",
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void RegisterViewModel_InvalidEmail_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_001 Test 1
            var model = new RegisterViewModel
            {
                Email = "invalidemail", // No @ symbol
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Nguyen Van A",
                Phone = "0912345678",
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_PasswordTooShort_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_001 Test 2
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "12345", // Less than 6 characters
                ConfirmPassword = "12345",
                FullName = "Nguyen Van A",
                Phone = "0912345678",
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void RegisterViewModel_PasswordMismatch_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_001 Test 3
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@654321", // Different password
                FullName = "Nguyen Van A",
                Phone = "0912345678",
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword"));
        }

        [Fact]
        public void RegisterViewModel_MissingRequiredField_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_001 Test 4
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = null, // Missing required field
                Phone = "0912345678",
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("FullName"));
        }

        [Fact]
        public void RegisterViewModel_InvalidPhone_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_001 Test 5
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Nguyen Van A",
                Phone = "abc123", // Invalid phone format
                Address = "123 ABC Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("Phone"));
        }

        #endregion

        #region LoginViewModel Tests

        [Fact]
        public void LoginViewModel_ValidData_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void LoginViewModel_MissingEmail_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_002 Test 1
            var model = new LoginViewModel
            {
                Email = null,
                Password = "Test@123456"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_PasswordTooShort_FailsValidation()
        {
            // Arrange - TC_WB_MODEL_002 Test 2
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "123" // Less than 6 characters
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion
    }
}
