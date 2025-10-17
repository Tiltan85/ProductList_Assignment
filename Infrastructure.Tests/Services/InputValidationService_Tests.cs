using Infrastructure.Models;
using Infrastructure.Services;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Tests.Services
{
    /*****

    AI GENERERAD KOD
    Lägger till kommentarer som förklarar.

    *****/

    public class InputValidationService_Tests
    {
        private readonly InputValidationService _service;

        public InputValidationService_Tests()
        {
            // Skapar en instans av InputValidationService (ingen mock behövs)
            _service = new InputValidationService();
        }

        // --- TESTER FÖR PRODUCT ---

        // Test för att verifiera VerifyProductForm, ska returnera Error om produkten är null
        [Fact]
        public void VerifyProductForm_Product_ShouldReturnError_WhenProductIsNull()
        {
            // Act
            // Skapar en null-produkt
            var result = _service.VerifyProductForm((Product)null!);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Equal("Product edit form is empty", result.Error); // rätt felmeddelande
        }

        // Test för att verifiera VerifyProductForm, ska returnera Error om fält saknas
        [Fact]
        public void VerifyProductForm_Product_ShouldReturnError_WhenFieldsMissing()
        {
            // Arrange
            // Skapar en produkt med alla fält tomma, priset är 0 som default för decimal
            var product = new Product
            {
                Id = "",
                ProductName = "",
                ProductDescription = "",
                Category = new Category { CategoryName = "" },
                Manufacturer = new Manufacturer { ManufacturerName = "" },
                ProductPrice = 0
            };

            // Act
            // Verifierar produkten
            var result = _service.VerifyProductForm(product);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Equal("Fields have errors.", result.Error); // rätt felmeddelande

            Assert.Contains(result.FieldErrors, e => e.Field == "Id"); // Fel på idfältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Name"); // Fel på namnfältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Description"); // Fel på beskrivningsfältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Category"); // Fel på kategorifältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Manufacturer"); // Fel på tillverkarfältet
        }

        // Test för att verifiera VerifyProductForm, ska returnera Error om priset är negativt
        [Fact]
        public void VerifyProductForm_Product_ShouldReturnError_WhenPriceNegative()
        {
            // Arrange
            // Skapar en produkt med negativt pris
            var product = new Product
            {
                Id = "1",
                ProductName = "Hammer",
                ProductDescription = "Strong tool",
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Bosch" },
                ProductPrice = -10
            };

            // Act
            // Verifierar produkten
            var result = _service.VerifyProductForm(product);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Contains(result.FieldErrors, e => e.Field == "Price"); // Fel på prisfältet
        }

        // Test för att verifiera VerifyProductForm, ska returnera Success om produkten är giltig
        [Fact]
        public void VerifyProductForm_Product_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            // Skapar en giltig produkt
            var product = new Product
            {
                Id = "1",
                ProductName = "Hammer",
                ProductDescription = "Strong tool",
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Bosch" },
                ProductPrice = 100
            };

            // Act
            // Verifierar produkten
            var result = _service.VerifyProductForm(product);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(204, result.StatusCode); // StatusCode ska vara 204
            Assert.Empty(result.FieldErrors); // Inga fältfel
        }

        // --- TESTER FÖR PRODUCTREQUEST ---

        // Test för att verifiera VerifyProductForm med ProductRequest, ska returnera Error om request är null
        [Fact]
        public void VerifyProductForm_Request_ShouldReturnError_WhenRequestIsNull()
        {
            // Act
            // Skapar en null-request
            var result = _service.VerifyProductForm((ProductRequest)null!);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Equal("Product edit form is empty", result.Error); // rätt felmeddelande
        }

        // Test för att verifiera VerifyProductForm med ProductRequest, ska returnera Error om fält saknas
        [Fact]
        public void VerifyProductForm_Request_ShouldReturnError_WhenFieldsMissing()
        {
            // Arrange
            // Skapar en request med alla fält tomma
            var request = new ProductRequest();

            // Act
            // Verifierar requesten
            var result = _service.VerifyProductForm(request);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Equal("Fields have errors.", result.Error); // rätt felmeddelande

            Assert.Contains(result.FieldErrors, e => e.Field == "Name"); // Fel på namnfältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Description"); // Fel på beskrivningsfältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Category"); // Fel på kategorifältet
            Assert.Contains(result.FieldErrors, e => e.Field == "Manufacturer"); // Fel på tillverkarfältet
        }

        // Test för att verifiera VerifyProductForm med ProductRequest, ska returnera Error om priset är negativt
        [Fact]
        public void VerifyProductForm_Request_ShouldReturnError_WhenPriceNegative()
        {
            // Arrange
            // Skapar en request med negativt pris
            var request = new ProductRequest
            {
                ProductName = "Saw",
                ProductDescription = "Sharp saw",
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Makita" },
                ProductPrice = -5
            };

            // Act
            // Verifierar requesten
            var result = _service.VerifyProductForm(request);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Contains(result.FieldErrors, e => e.Field == "Price"); // Fel på prisfältet
        }

        // Test för att verifiera VerifyProductForm med ProductRequest, ska returnera Success om request är giltig
        [Fact]
        public void VerifyProductForm_Request_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            // Skapar en giltig request
            var request = new ProductRequest
            {
                ProductName = "Drill",
                ProductDescription = "Cordless drill",
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Makita" },
                ProductPrice = 999
            };

            // Act
            // Verifierar requesten
            var result = _service.VerifyProductForm(request);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(204, result.StatusCode); // StatusCode ska vara 204
            Assert.Empty(result.FieldErrors); // Inga fältfel
        }
    }
}