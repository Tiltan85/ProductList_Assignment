using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class ProductService_Tests
    {
        private readonly Mock<IJsonFileRepository> _jsonMock; // Lagrar en Mock av IJsonFileRepository
        private readonly Mock<IInputValidationService> _validationMock;//  Lagrar en Mock av IInputValidationService
        private readonly ProductService _service; // Lagrar en ProductService
        private readonly List<Product> _sampleProducts; // Lagrar en lista av typen  Products

        public ProductService_Tests()
        {
            _jsonMock = new Mock<IJsonFileRepository>(); // Skapar en mockad version av IJsonFileRepository
            _validationMock = new Mock<IInputValidationService>(); // Skapar en mockad version av IInputValidationService

            // Skapar en ny lista med 2 testprodukter
            _sampleProducts = new List<Product>
            {
                new() { Id = "1", ProductName = "Hammer", ProductPrice = 100, ProductDescription = "Heavy hammer" },
                new() { Id = "2", ProductName = "Saw", ProductPrice = 150, ProductDescription = "Sharp saw" }
            };

            // Mockar ReadAsync, läser in listan med produkter ovan när den används 
            _jsonMock.Setup(repo => repo.ReadAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(_sampleProducts);

            // Skapar en ny ProductService med mockade JsonFileRepository och InputValidationService
            _service = new ProductService(_jsonMock.Object, _validationMock.Object);
        }

        // test för att verifiera GetProductAsync, ska returnera en lista med alla produkter
        [Fact]
        public async Task GetProductAsync_ShouldReturnAllProducts()
        {
            // Act
            // Hämtar listan med produkter
            var result = await _service.GetProductAsync();

            // Assert
            Assert.True(result.Success);    // Success ska vara true
            Assert.Equal(200, result.StatusCode); // StatusCode ska vara 200
            Assert.Equal(2, result.Content?.Count); // Content ska innehålla 2 produkter i listan
        }

        // Test för att verifiera att GetProductByIdAsync returnerar rätt produkt efter Id
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            // Hämtar listan med produkter
            await _service.GetProductAsync(); // load products

            // Act
            // Söker i listan efter en produkt med Id 1
            var result = await _service.GetProductByIdAsync("1");

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal("Hammer", result.Content?.ProductName); // ProductName ska vara "Hammer"
        }

        // test för att verifiera att GetProductByIdAsync returnerar 404 om den inte hittar en produkt
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturn404_WhenNotFound()
        {
            // Arrange
            // Hämtar listan med produkter
            await _service.GetProductAsync();

            // Act
            // Söker i listan efter en produkt med Id 999
            var result = await _service.GetProductByIdAsync("999");

            // Assert
            Assert.False(result.Success); //Success ska vara false
            Assert.Equal(404, result.StatusCode); // StatusCode ska vara 404
        }

        // Test för att verifiera att GetProductByNameAsync returnerar rätt produkt
        [Fact]
        public async Task GetProductByNameAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            // Hämtar listan med produkter
            await _service.GetProductAsync();

            // Act
            // Söker i listan efter en produkt med namnet Saw
            var result = await _service.GetProductByNameAsync("Saw");

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal("2", result.Content?.Id); // Content.Id ska vara 2 efter som att Saw har id 2
        }

        // Test för att verifiera att GetProductByNameAsync returnerar 404 om den inte hittar en produkt
        [Fact]
        public async Task GetProductByNameAsync_ShouldReturn404_WhenNotFound()
        {
            // Arrange
            // Hämtar listan med produkter
            await _service.GetProductAsync();

            // Act
            // Söker i listan efter en produkt med namnet Drill
            var result = await _service.GetProductByNameAsync("Drill");

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(404, result.StatusCode); // Statuscode ska vara 404
        }

        // Test för att verifiera att SaveProductAsync lägger till en ny produkt när det är en giltig produkt
        [Fact]
        public async Task SaveProductAsync_ShouldAddNewProduct_WhenProductIsValid()
        {
            // Arrange
            // Mock på en giltig produkt verifiering
            _validationMock.Setup(v => v.VerifyProductForm(It.IsAny<ProductRequest>()))
                           .Returns(new ServiceResult { Success = true });

            // Mock på att produkten sparades i filen
            _jsonMock.Setup(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Skapar en ny produkt
            var newRequest = new ProductRequest
            {
                ProductName = "Drill",
                ProductPrice = 200,
                ProductDescription = "Cordless drill",
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Makita" }
            };

            // Act
            // simulerar att produkten sparas med dom mockade verifiering och sparning Setup ovan.
            var result = await _service.SaveProductAsync(newRequest);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(204, result.StatusCode); //StatusCode ska vara 204

            // Verifierar att WriteAsync kallades en gång
            _jsonMock.Verify(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // test för att verifiera att SaveproductAsync returnerar Error när validering för en produkt inte är rätt
        [Fact]
        public async Task SaveProductAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            // Mock på en ogiltig produkt verifiering
            _validationMock.Setup(v => v.VerifyProductForm(It.IsAny<ProductRequest>()))
                           .Returns(new ServiceResult { Success = false, Error = "Invalid form" });

            // Act
            // simulerar att produkten sparas med dom mockade verifiering Setup ovan.
            var result = await _service.SaveProductAsync(new ProductRequest());

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal("Invalid form", result.Error); // Error ska vara "Invalid form"

            // Verifierar att WriteAsync aldrig kallades eftersom valideringen misslyckades
            _jsonMock.Verify(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // test för att verifiera att EditProductAsync uppdaterar en existerande produkt
        [Fact]
        public async Task EditProductAsync_ShouldUpdateExistingProduct()
        {
            // Arrange
            // Hämtar lisan med produkter
            await _service.GetProductAsync();

            // Skapar en produkt med Id 1 som ska uppdateras
            var productToEdit = new Product
            {
                Id = "1",
                ProductName = "Hammer Pro",
                ProductDescription = "Updated desc",
                ProductPrice = 120,
                Category = new Category { CategoryName = "Tools" },
                Manufacturer = new Manufacturer { ManufacturerName = "Bosch" }
            };

            // Mock på en giltig produkt verifiering
            _validationMock.Setup(v => v.VerifyProductForm(It.IsAny<Product>()))
                           .Returns(new ServiceResult { Success = true });

            // Mock på att produkten sparades i filen
            _jsonMock.Setup(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            // simulerar att produkten uppdateras med dom mockade verifiering och sparning Setup ovan.
            var result = await _service.EditProductAsync(productToEdit);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(204, result.StatusCode); // StatusCode ska vara 204

            // Verifierar att WriteAsync kallades en gång
            _jsonMock.Verify(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        // test för att verifiera att EditProductAsync returnerar 404 när produkten inte finns
        public async Task EditProductAsync_ShouldReturn404_WhenNotFound()
        {
            // Arrange
            // Mock på en giltig produkt verifiering
            _validationMock.Setup(v => v.VerifyProductForm(It.IsAny<Product>()))
                           .Returns(new ServiceResult { Success = true });

            // Skapar en produkt med ett icke existerande Id och namn
            var productToEdit = new Product { Id = "999", ProductName = "Ghost" };

            // Act
            // simulerar att produkten uppdateras med dom mockade verifiering Setup ovan.
            var result = await _service.EditProductAsync(productToEdit);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(404, result.StatusCode); // StatusCode ska vara 404
        }


        // test för att verifiera att DeleteProductAsync tar bort en produkt
        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProduct()
        {
            // Arrange
            // Hämtar listan med produkter
            await _service.GetProductAsync();

            // Mock på att produkten sparades i filen
            _jsonMock.Setup(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            // simulerar att produkten tas bort
            var result = await _service.DeleteProductAsync(_sampleProducts[0]);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(200, result.StatusCode); // StatusCode ska vara 200

            // Verifierar att WriteAsync kallades en gång
            _jsonMock.Verify(repo => repo.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        // test för att verifiera att DeleteProductAsync returnerar error när produkten är null
        [Fact]
        public async Task DeleteProductAsync_ShouldReturnError_WhenProductNull()
        {
            // Act
            // simulerar att produkten tas bort med en null produkt
            var result = await _service.DeleteProductAsync(null!);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(500, result.StatusCode); // StatusCode ska vara 500
        }


        // test för att verifiera ProductNotExistCheck returnerar fel när det finns en namn konflikt
        [Fact]
        public async Task ProductNotExistCheck_ShouldDetectNameConflict()
        {
            // Arrange
            // Hämtar listan med produkter
            // Fick lägga till För AI gjorde fel!!! testet gick inte igenom utan denna rad
            await _service.GetProductAsync();

            // Produkt med ett namn som redan finns i _sampleProducts
            var conflictProduct = new Product { Id = "3", ProductName = "Hammer" };

            // Act
            // Kollar om det finns en namn konflikt
            var result = _service.ProductNotExistCheck(conflictProduct);

            // Assert
            Assert.False(result.Success); // Success ska vara false
            Assert.Equal(400, result.StatusCode); // StatusCode ska vara 400
            Assert.Contains(result.FieldErrors, e => e.Field == "Name"); // FieldErrors ska innehålla en fel för "Name"
        }


        // test för att verifiera ProductNotExistCheck returnerar success när produkten är unik
        [Fact]
        public void ProductNotExistCheck_ShouldReturnSuccess_WhenUnique()
        {
            // Arrange
            // Produkt med ett unikt namn
            var unique = new Product { Id = "99", ProductName = "NewTool" };

            // Act
            // Kollar om produkten är unik
            var result = _service.ProductNotExistCheck(unique);

            // Assert
            Assert.True(result.Success); // Success ska vara true
            Assert.Equal(204, result.StatusCode); // StatusCode ska vara 204
        }
    }
}
