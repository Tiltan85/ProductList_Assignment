
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public interface IProductService
{
    Task<ProductObjectResult<IReadOnlyList<Product>>> GetProductAsync(CancellationToken cancellationToken = default);
    Task<ProductObjectResult<Product>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);
    Task<ProductObjectResult<Product>> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default);
    Task<ProductResult> SaveProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default);
}

public class ProductService(IJsonFileRepository jsonFileRepository) : IProductService
{
    private readonly IJsonFileRepository _jsonFileRepository = jsonFileRepository;
    private List<Product> _products = [];
    private bool _loaded;

    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_loaded) return;

        var result = await _jsonFileRepository.ReadAsync(cancellationToken);
        _products = [.. result];

        _loaded = true;
    }

    public async Task<ProductObjectResult<IReadOnlyList<Product>>> GetProductAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);
        return new ProductObjectResult<IReadOnlyList<Product>>
        {
            Success = true,
            StatusCode = 200,
            Content = _products.AsReadOnly(),
        };
    }

    public async Task<ProductObjectResult<Product>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);
        var product = _products.FirstOrDefault(product => product.Id == productId);

        if (product == null)
            return new ProductObjectResult<Product>
            {
                Success = true,
                StatusCode = 404,
                Error = "Product Not Found"
            };

        return new ProductObjectResult<Product>
        {
            Success = true,
            StatusCode = 200,
            Content = product,
        };
    }

    public async Task<ProductObjectResult<Product>> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);
        var product = _products.FirstOrDefault(product => product.ProductName == productName);

        if (product == null)
            return new ProductObjectResult<Product> 
            {   Success = true, 
                StatusCode = 404, // 404 Not Found
                Error = "Product Not Found" 
            };

        return new ProductObjectResult<Product>
        {
            Success = true,
            StatusCode = 200, // Allt gick som det ska, skickar tillbaka produkt
            Content = product,
        };
    }

    public async Task<ProductResult> SaveProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default)
    {
        if (productRequest == null)
            return new ProductResult { Success = false, StatusCode = 400, Error = "Invalid Input" }; // 400 Bad Request, Fel input från användaren

        if (string.IsNullOrWhiteSpace(productRequest.ProductName))
            return new ProductResult { Success = false, StatusCode = 400, Error = "Invalid product name" };
        // TODO Lägg till Description, Brand

        try
        {
            // EnsureLoaded efter kontroll om giltig input
            await EnsureLoadedAsync(cancellationToken);

            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                ProductName = productRequest.ProductName,
                ProductPrice = productRequest.ProductPrice,
            };

            _products.Add(product);
            await _jsonFileRepository.WriteAsync(_products, cancellationToken);

            return new ProductResult { Success = true, StatusCode = 204 }; // 204 Allt gick som det ska, skickar inte tillbaka något
        }
        catch (Exception ex)
        {
            return new ProductResult { Success = false, StatusCode = 500, Error = ex.Message }; // 500 generellt felmeddelande.
        }

    }
}
