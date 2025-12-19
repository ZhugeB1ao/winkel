using Xunit;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.BlackBox
{
    /// <summary>
    /// Black Box Tests for RegisterViewModel Validation
    /// Techniques: Equivalence Partitioning, Boundary Value Analysis, Decision Table
    /// Test Cases: TC_BB_REG_001 - TC_BB_REG_010
    /// </summary>
    public class RegisterViewModelBlackBoxTests
    {
        #region Helper Methods
        
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        private RegisterViewModel GetValidModel()
        {
            return new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123456",
                ConfirmPassword = "Test@123456",
                FullName = "Test User",
                Phone = "0912345678",
                Address = "123 Test Street"
            };
        }

        #endregion

        #region TC_BB_REG_001: EP - Valid Email Format

        /// <summary>
        /// TC_BB_REG_001: Test with valid email formats
        /// Equivalence Partitioning: Valid email class
        /// </summary>
        [Theory]
        [InlineData("user@domain.com")]
        [InlineData("user.name@domain.com")]
        [InlineData("user@sub.domain.com")]
        [InlineData("user123@domain.org")]
        public void Email_ValidFormats_PassValidation(string email)
        {
            // Arrange
            var model = GetValidModel();
            model.Email = email;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_BB_REG_002: EP - Invalid Email Format

        /// <summary>
        /// TC_BB_REG_002: Test with invalid email formats
        /// Equivalence Partitioning: Invalid email class
        /// </summary>
        [Theory]
        [InlineData("plaintext")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        public void Email_InvalidFormats_FailValidation(string email)
        {
            // Arrange
            var model = GetValidModel();
            model.Email = email;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        #endregion

        #region TC_BB_REG_003: BVA - Password Minimum Length (6)

        /// <summary>
        /// TC_BB_REG_003: Test password at minimum length
        /// Boundary Value Analysis: Password length = 6
        /// </summary>
        [Fact]
        public void Password_AtMinimumLength_PassesValidation()
        {
            // Arrange
            var model = GetValidModel();
            model.Password = "123456"; // Exactly 6 chars
            model.ConfirmPassword = "123456";
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_REG_004: BVA - Password Below Minimum (5)

        /// <summary>
        /// TC_BB_REG_004: Test password below minimum length
        /// Boundary Value Analysis: Password length = 5
        /// </summary>
        [Fact]
        public void Password_BelowMinimum_FailsValidation()
        {
            // Arrange
            var model = GetValidModel();
            model.Password = "12345"; // 5 chars
            model.ConfirmPassword = "12345";
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_REG_005: BVA - Password Maximum Length (100)

        /// <summary>
        /// TC_BB_REG_005: Test password at maximum length
        /// Boundary Value Analysis: Password length = 100
        /// </summary>
        [Fact]
        public void Password_AtMaximumLength_PassesValidation()
        {
            // Arrange
            var password = new string('a', 100);
            var model = GetValidModel();
            model.Password = password;
            model.ConfirmPassword = password;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_REG_006: BVA - Password Above Maximum (101)

        /// <summary>
        /// TC_BB_REG_006: Test password above maximum length
        /// Boundary Value Analysis: Password length = 101
        /// </summary>
        [Fact]
        public void Password_AboveMaximum_FailsValidation()
        {
            // Arrange
            var password = new string('a', 101);
            var model = GetValidModel();
            model.Password = password;
            model.ConfirmPassword = password;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        #endregion

        #region TC_BB_REG_007: DT - Password Match Decision

        /// <summary>
        /// TC_BB_REG_007: Test password confirmation mismatch
        /// Decision Table: Password != ConfirmPassword
        /// </summary>
        [Fact]
        public void ConfirmPassword_Mismatch_FailsValidation()
        {
            // Arrange
            var model = GetValidModel();
            model.Password = "Password123";
            model.ConfirmPassword = "DifferentPassword";
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword"));
        }

        #endregion

        #region TC_BB_REG_008: EP - Valid Phone Formats

        /// <summary>
        /// TC_BB_REG_008: Test valid phone number formats
        /// Equivalence Partitioning: Valid phone class
        /// </summary>
        [Theory]
        [InlineData("0912345678")]
        [InlineData("+84912345678")]
        [InlineData("84-912-345-678")]
        public void Phone_ValidFormats_PassValidation(string phone)
        {
            // Arrange
            var model = GetValidModel();
            model.Phone = phone;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Phone"));
        }

        #endregion

        #region TC_BB_REG_009: EP - Invalid Phone Formats

        /// <summary>
        /// TC_BB_REG_009: Test invalid phone number formats
        /// Equivalence Partitioning: Invalid phone class
        /// </summary>
        [Theory]
        [InlineData("abcdefghij")]
        [InlineData("phone")]
        public void Phone_InvalidFormats_FailValidation(string phone)
        {
            // Arrange
            var model = GetValidModel();
            model.Phone = phone;
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Phone"));
        }

        #endregion

        #region TC_BB_REG_010: DT - All Fields Valid Decision

        /// <summary>
        /// TC_BB_REG_010: Test all fields valid passes validation
        /// Decision Table: All conditions true
        /// </summary>
        [Fact]
        public void AllFieldsValid_PassesValidation()
        {
            // Arrange
            var model = GetValidModel();
            
            // Act
            var results = ValidateModel(model);
            
            // Assert
            Assert.Empty(results);
        }

        #endregion
    }
}
