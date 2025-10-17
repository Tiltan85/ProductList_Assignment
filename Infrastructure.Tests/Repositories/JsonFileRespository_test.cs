using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
    /*****

    AI GENERATED CODE
    Lägger till kommentarer som förklarar.

    *****/


    // Klassen ärver från IDisposable för att kunna städa upp resurser efter att testerna körts.
    // IDisposable används ofta i testklasser för att radera temporära filer, stänga anslutningar, mm.
    // Jag bad AI förklara vad IDisposable gör eftersom det var nytt för mig.
    public class JsonFileRepository_Tests : IDisposable
    {
        private readonly string _testDirectory; // Lagrar testmappen
        private readonly string _testFilePath; // Largrar testfilen
        private readonly JsonFileRepository _repository;

        public JsonFileRepository_Tests()
        {
            // Skapa en temporär mapp för tester
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Skapar sökväg till testfilen
            _testFilePath = Path.Combine(_testDirectory, "products_test.json");

            // Initiera repository med filnamnet
            _repository = new JsonFileRepository("products_test.json");

            // Försäkrar att testfilen ligger i testmappen för att inte råka ändra i riktiga filer.
            var dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
            // Om "Data" mappen finns
            if (Directory.Exists(dataFolder))
            {
                // Anger den ursprungliga sökvägen till testfilen
                var source = Path.Combine(dataFolder, "products_test.json");
                // Om filen finns i Data mappen 
                if (File.Exists(source))
                { 
                    // Flytta filen till testmappen och skriv över om den redan finns där.
                    File.Move(source, _testFilePath, true);
                }
            }
        }

        // AI Förklarade denna metoden eftersom att jag inte visste var den kördes
        // Dispose() körs automatiskt efter att testklassen använts färdigt av test-ramverket (xUnit)
        // Här rensar vi bort temporära filer och mappar så att inget skräp lämnas kvar.
        public void Dispose()
        {
            if (Directory.Exists(_testDirectory)) // Om _testDirectory finns
                Directory.Delete(_testDirectory, true); // Tar bort _testDirectory, true betyder: ta bort även undermappar och filer
        }

        // ---------------------------
        // TESTER FÖR EnsureInitialized
        // ---------------------------

        // test för att verifiera EnsureInitialized skapar en mapp och fil om dom inte finns
        [Fact]
        public void EnsureInitialized_ShouldCreateDirectoryAndFile()
        {
            // Arrange
            var dir = Path.Combine(_testDirectory, "EnsureInitDir"); // Skapar sökväg till testmappen
            var file = Path.Combine(dir, "test.json"); // skapar sökväg till filen i testmapepen

            // Act
            // Skapar mapp och fil
            JsonFileRepository.EnsureInitialized(dir, file);

            // Assert
            Assert.True(Directory.Exists(dir)); // Kontrollerar så att mappen finns
            Assert.True(File.Exists(file)); // Kontrollerar så att filen finns
            var content = File.ReadAllText(file); // Läser filen
            Assert.Equal("[]", content); // Kontrollerar att content en tom Json lista
        }

        // ---------------------------
        // TESTER FÖR WriteAsync & ReadAsync
        // ---------------------------

        // Test för att verifiera att WriteAsync sparar produkter till fil och att ReadAsync kan läsa filen.
        [Fact]
        public async Task WriteAsync_And_ReadAsync_ShouldPersistProducts()
        {
            // Arrange
            // Skapar en lista med 2 produkter
            var products = new List<Product>
            {
                new() { Id = "1", ProductName = "Hammer", ProductDescription = "Steel hammer", ProductPrice = 100 },
                new() { Id = "2", ProductName = "Screwdriver", ProductDescription = "Flat head", ProductPrice = 50 }
            };

            // Skapar ett nytt JsonFileRepoitory och anger vilken fil som ska användas
            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            await repo.WriteAsync(products); // Skriver in listan med produkter till filen
            var result = await repo.ReadAsync(); // läser listan från filen

            // Assert
            Assert.NotNull(result); // Kontrollerar så att resultatet inte är null
            Assert.Equal(2, result.Count); // Kontrollerar att där är 2 produkter i listan
            Assert.Equal("Hammer", result[0].ProductName); // Kontrollera så att första resultatet är Hammer
            Assert.Equal("Screwdriver", result[1].ProductName); // Kontrollera så att andra resultatet är Screwdriver
        }

        // Test för att verifiera att när filen är tom så ska den returnera en tom lista
        [Fact]
        public async Task ReadAsync_WhenFileIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            // Skapar en tom fil
            await File.WriteAllTextAsync(_testFilePath, "");

            // Skapar ett nytt JsonFileRepoitory och anger vilken fil som ska användas
            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            // läser filen
            var result = await repo.ReadAsync();

            // Assert
            Assert.NotNull(result); // Kontrollerar så att resultatet inte är null
            Assert.Empty(result); // Kontrollerar så att resultatet är tomt
        }

        // test för att verifiera att ReadAsync hanterar om filen innehåller ogiltig Json, ska returnera tom Json lista.
        [Fact]
        public async Task ReadAsync_WhenFileContainsInvalidJson_ShouldReturnEmptyList()
        {
            // Arrange
            // skapar en fil som inehåller ogiltig json 
            await File.WriteAllTextAsync(_testFilePath, "{invalid json}");

            // Skapar ett nytt JsonFileRepoitory och anger vilken fil som ska användas
            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Act
            // läser filen
            var result = await repo.ReadAsync();

            // Assert
            Assert.NotNull(result); // Kontrollerar så att resultatet inte är null
            Assert.Empty(result); // Kontrollerar så att resultatet är tomt
        }

        // test för att verifiera så WriteAsync skriver över befintlig fil
        [Fact]
        public async Task WriteAsync_ShouldOverwriteExistingFile()
        {
            // Arrange
            // Skapar 2 olika listor med en produkter för att simulera överskrivning i filen
            var initial = new List<Product> { new() { Id = "1", ProductName = "OldProduct" } };
            var updated = new List<Product> { new() { Id = "2", ProductName = "NewProduct" } };

            // Skapar ett nytt JsonFileRepoitory och anger vilken fil som ska användas
            var repo = new JsonFileRepository(Path.GetFileName(_testFilePath));

            // Skriver in "initial" listan till filen
            await repo.WriteAsync(initial);

            // Act
            // skriver över filen med updated listan
            await repo.WriteAsync(updated);
            var result = await repo.ReadAsync(); // Läser filen

            // Assert
            Assert.Single(result); // Där ska vara ett resultat eftersom initial skrevs över av updated
            Assert.Equal("NewProduct", result[0].ProductName); // det nya värdet ska vara NewProduct
        }
    }
}
