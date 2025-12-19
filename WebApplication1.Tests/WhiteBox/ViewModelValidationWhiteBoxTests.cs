using Xunit;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.WhiteBox
{
    /// <summary>
    /// White Box Tests for ViewModel Validation
    /// Testing validation logic and rules
    /// Test Cases: TC_WB_MODEL_001 - TC_WB_MODEL_010
    /// </summary>
    public class ViewModelValidationWhiteBoxTests
    {
        #region Helper Methods
        
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion

        #region TC_WB_MODEL_001: RegisterViewModel - Required Email Validation

        /// <summary>
        /// TC_WB_MODEL_001: Test Required attribute on Email
        /// Condition Coverage: [Required] validation
        /// </summary>
        [Fact]
        public void RegisterViewModel_MissingEmail_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = null!,
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_WB_MODEL_002: RegisterViewModel - EmailAddress Validation

        /// <summary>
        /// TC_WB_MODEL_002: Test [EmailAddress] attribute
        /// Condition Coverage: Email format validation
        /// </summary>
        [Fact]
        public void RegisterViewModel_InvalidEmailFormat_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "notanemail",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_WB_MODEL_003: RegisterViewModel - Password StringLength Min

        /// <summary>
        /// TC_WB_MODEL_003: Test [StringLength] MinimumLength = 6
        /// Boundary: Password length < 6
        /// </summary>
        [Fact]
        public void RegisterViewModel_PasswordTooShort_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "12345", // 5 chars, below minimum
                ConfirmPassword = "12345",
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_WB_MODEL_004: RegisterViewModel - Password StringLength Max

        /// <summary>
        /// TC_WB_MODEL_004: Test [StringLength] maximum = 100
        /// Boundary: Password length > 100
        /// </summary>
        [Fact]
        public void RegisterViewModel_PasswordTooLong_FailsValidation()
        {
            // Arrange
            var password = new string('a', 101); // 101 chars, above maximum
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = password,
                ConfirmPassword = password,
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_WB_MODEL_005: RegisterViewModel - Compare Password Validation

        /// <summary>
        /// TC_WB_MODEL_005: Test [Compare("Password")] attribute
        /// Condition Coverage: ConfirmPassword must match Password
        /// </summary>
        [Fact]
        public void RegisterViewModel_PasswordMismatch_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "DifferentPassword",
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword"));
        }

        #endregion

        #region TC_WB_MODEL_006: RegisterViewModel - Phone Validation

        /// <summary>
        /// TC_WB_MODEL_006: Test [Phone] attribute
        /// Condition Coverage: Phone format validation
        /// </summary>
        [Fact]
        public void RegisterViewModel_InvalidPhone_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Test User",
                Phone = "notaphone",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Phone"));
        }

        #endregion

        #region TC_WB_MODEL_007: RegisterViewModel - Required FullName

        /// <summary>
        /// TC_WB_MODEL_007: Test [Required] on FullName
        /// Condition Coverage: FullName must not be null
        /// </summary>
        [Fact]
        public void RegisterViewModel_MissingFullName_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = null!,
                Phone = "0912345678",
                Address = "123 Test Street"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FullName"));
        }

        #endregion

        #region TC_WB_MODEL_008: RegisterViewModel - Required Address

        /// <summary>
        /// TC_WB_MODEL_008: Test [Required] on Address
        /// Condition Coverage: Address must not be null
        /// </summary>
        [Fact]
        public void RegisterViewModel_MissingAddress_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Test User",
                Phone = "0912345678",
                Address = null!
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Address"));
        }

        #endregion

        #region TC_WB_MODEL_009: LoginViewModel - Required Email

        /// <summary>
        /// TC_WB_MODEL_009: Test [Required] on LoginViewModel Email
        /// Condition Coverage: Email must not be null
        /// </summary>
        [Fact]
        public void LoginViewModel_MissingEmail_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = null!,
                Password = "Test@123456"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_WB_MODEL_010: LoginViewModel - Required Password

        /// <summary>
        /// TC_WB_MODEL_010: Test [Required] on LoginViewModel Password
        /// Condition Coverage: Password must not be null
        /// </summary>
        [Fact]
        public void LoginViewModel_MissingPassword_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = null!
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion
    }
}
