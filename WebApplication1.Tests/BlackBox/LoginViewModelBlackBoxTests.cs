using Xunit;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.BlackBox
{
    /// <summary>
    /// Black Box Tests for LoginViewModel Validation
    /// Techniques: Equivalence Partitioning, Boundary Value Analysis, Decision Table
    /// Test Cases: TC_BB_LOGIN_001 - TC_BB_LOGIN_010
    /// </summary>
    public class LoginViewModelBlackBoxTests
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

        #region TC_BB_LOGIN_001: EP - Valid Credentials Format

        /// <summary>
        /// TC_BB_LOGIN_001: Test with valid email and password
        /// Equivalence Partitioning: Valid credentials class
        /// </summary>
        [Fact]
        public void Login_ValidCredentials_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "validpass123"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Empty(results);
        }

        #endregion

        #region TC_BB_LOGIN_002: EP - Missing Email

        /// <summary>
        /// TC_BB_LOGIN_002: Test with missing email
        /// Equivalence Partitioning: Missing email class
        /// </summary>
        [Fact]
        public void Login_MissingEmail_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "",
                Password = "validpass123"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_BB_LOGIN_003: EP - Missing Password

        /// <summary>
        /// TC_BB_LOGIN_003: Test with missing password
        /// Equivalence Partitioning: Missing password class
        /// </summary>
        [Fact]
        public void Login_MissingPassword_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = ""
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_LOGIN_004: DT - Both Fields Missing

        /// <summary>
        /// TC_BB_LOGIN_004: Test with both fields missing
        /// Decision Table: Email empty AND Password empty
        /// </summary>
        [Fact]
        public void Login_BothFieldsMissing_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "",
                Password = ""
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_LOGIN_005: EP - Invalid Email Format

        /// <summary>
        /// TC_BB_LOGIN_005: Test with invalid email format
        /// Equivalence Partitioning: Invalid email class
        /// </summary>
        [Theory]
        [InlineData("notanemail")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        public void Login_InvalidEmailFormat_FailsValidation(string email)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = email,
                Password = "validpass123"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_BB_LOGIN_006: BVA - Password At Minimum Length

        /// <summary>
        /// TC_BB_LOGIN_006: Test password at minimum length (6)
        /// Boundary Value Analysis: Minimum length
        /// </summary>
        [Fact]
        public void Login_PasswordAtMinLength_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "123456" // 6 chars
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_LOGIN_007: BVA - Password Below Minimum

        /// <summary>
        /// TC_BB_LOGIN_007: Test password below minimum length (5)
        /// Boundary Value Analysis: Below minimum
        /// </summary>
        [Fact]
        public void Login_PasswordBelowMinLength_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "12345" // 5 chars
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_LOGIN_008: EP - Valid Email Formats

        /// <summary>
        /// TC_BB_LOGIN_008: Test various valid email formats
        /// Equivalence Partitioning: Valid email class
        /// </summary>
        [Theory]
        [InlineData("simple@example.com")]
        [InlineData("very.common@example.com")]
        [InlineData("x@example.com")]
        public void Login_ValidEmailFormats_PassValidation(string email)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = email,
                Password = "validpass123"
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_BB_LOGIN_009: BVA - Password At Maximum Length

        /// <summary>
        /// TC_BB_LOGIN_009: Test password at maximum length (100)
        /// Boundary Value Analysis: Maximum length
        /// </summary>
        [Fact]
        public void Login_PasswordAtMaxLength_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = new string('a', 100)
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_LOGIN_010: BVA - Password Above Maximum

        /// <summary>
        /// TC_BB_LOGIN_010: Test password above maximum length (101)
        /// Boundary Value Analysis: Above maximum
        /// </summary>
        [Fact]
        public void Login_PasswordAboveMaxLength_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = new string('a', 101)
            };
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion
    }
}
