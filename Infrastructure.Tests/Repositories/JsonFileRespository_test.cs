using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
    /*****

    AI GENERATED CODE
    Lägger till kommentarer som förklarar.

    *****/

    public class JsonFileRepository_Tests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;
        private readonly JsonFileRepository _repository;

        public JsonFileRepository_Tests()
        {
            // Skapa en temporär mapp för tester
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _testFilePath = Path.Combine(_testDirectory, "products_test.json");

            // Initiera repository med filnamnet och skapa datamapp i testkatalogen
            _repository = new JsonFileRepository("products_test.json");

            // Flytta filen till testkatalogen för att inte röra verkliga filer
            var dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
            if (Directory.Exists(dataFolder))
            {
                var src = Path.Combine(dataFolder, "products_test.json");
                if (File.Exists(src))
                {
                    File.Move(src, _testFilePath, true);
                }
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        // ---------------------------
        // TESTER FÖR EnsureInitialized
        // ---------------------------

        [Fact]
        public void EnsureInitialized_ShouldCreateDirectoryAndFile()
        {
            // Arrange
            var dir = Path.Combine(_testDirectory, "EnsureInitDir");
            var file = Path.Combine(dir, "test.json");

            // Act
            JsonFileRepository.EnsureInitialized(dir, file);

            // Assert
            Assert.True(Directory.Exists(dir));
            Assert.True(File.Exists(file));
            var content = File.ReadAllText(file);
            Assert.Equal("[]", content);
        }

        // ---------------------------
        // TESTER FÖR WriteAsync & ReadAsync
        // ---------------------------

        [Fact]
        public async Task WriteAsync_And_ReadAsync_ShouldPersistProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = "1", ProductName = "Hammer", ProductDescription = "Steel hammer", ProductPrice = 100 },
                new() { Id = "2", ProductName = "Screwdriver", ProductDescription = "Flat head", ProductPrice = 50 }
            };

            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            await repo.WriteAsync(products);
            var result = await repo.ReadAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Hammer", result[0].ProductName);
            Assert.Equal("Screwdriver", result[1].ProductName);
        }

        [Fact]
        public async Task ReadAsync_WhenFileIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "");

            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            var result = await repo.ReadAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ReadAsync_WhenFileContainsInvalidJson_ShouldReturnEmptyList()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFilePath, "{invalid json}");

            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            var result = await repo.ReadAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task WriteAsync_ShouldOverwriteExistingFile()
        {
            // Arrange
            var initial = new List<Product> { new() { Id = "1", ProductName = "OldProduct" } };
            var updated = new List<Product> { new() { Id = "2", ProductName = "NewProduct" } };

            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            await repo.WriteAsync(initial);

            // Act
            await repo.WriteAsync(updated);
            var result = await repo.ReadAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("NewProduct", result[0].ProductName);
        }
    }
}
