
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public interface IProductService
{
    Task<ProductObjectResult<IReadOnlyList<Product>>> GetProductAsync(CancellationToken cancellationToken = default);
    Task<ProductObjectResult<Product>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);
    Task<ProductObjectResult<Product>> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default);
    Task<ProductResult> SaveProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default);
    Task<ProductResult> EditProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<ProductResult> DeleteProductAsync(Product product, CancellationToken cancellationToken = default);
}

public class ProductService(IJsonFileRepository jsonFileRepository, IInputValidationService inputValidationService) : IProductService
{
    private readonly IJsonFileRepository _jsonFileRepository = jsonFileRepository;
    private readonly IInputValidationService _inputValidationService = inputValidationService;
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
        var result = _inputValidationService.VerifyProductForm(productRequest);

        if (result.Success)
        {
            try
            {
                // EnsureLoaded efter kontroll om giltig input. Onödigt att hämta om man har fel i formuläret
                await EnsureLoadedAsync(cancellationToken);

                var product = new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductName = productRequest.ProductName,
                    ProductPrice = productRequest.ProductPrice,
                    ProductDescription = productRequest.ProductDescription,
                    Category = new Category { CategoryName = productRequest.Category },
                    Manufacturer = new Manufacturer { ManufacturerName = productRequest.Manufacturer },
                };

                // TODO ProductDuplicationCheck (kolla så där inte redan finns en product med samma Id, och namn)

                _products.Add(product);
                await _jsonFileRepository.WriteAsync(_products, cancellationToken);

                return new ProductResult { Success = true, StatusCode = 204 }; // 204 Allt gick som det ska, skickar inte tillbaka något
            }
            catch (Exception ex)
            {
                return new ProductResult { Success = false, StatusCode = 500, Error = ex.Message }; // 500 generellt felmeddelande.
            }
        }
        return new ProductResult { Success = false, StatusCode = result.StatusCode, Error = result.Error, FieldErrors = result.FieldErrors };
    }

    public async Task<ProductResult> EditProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var result = _inputValidationService.VerifyProductForm(product);

        if (result.Success)
        {
            try
            {
                var existingProduct = await GetProductByIdAsync(product.Id, cancellationToken);

                if (existingProduct.Content is not null)
                {
                    existingProduct.Content.ProductName = product.ProductName;
                    existingProduct.Content.ProductDescription = product.ProductDescription;
                    existingProduct.Content.Category = product.Category;
                    existingProduct.Content.Manufacturer = product.Manufacturer;
                    existingProduct.Content.ProductPrice = product.ProductPrice;

                    // TODO ProductDuplicationCheck (kolla så där inte redan finns en product med samma Id, och namn)

                    await EnsureLoadedAsync(cancellationToken);
                    await _jsonFileRepository.WriteAsync(_products, cancellationToken);

                    return new ProductResult { Success = true, StatusCode = 204 }; // 204 Allt gick som det ska, skickar inte tillbaka något
                }
                return new ProductResult { Success = false, StatusCode = 404, Error = "Product not found" };

            }
            catch (Exception ex)
            {
                return new ProductResult { Success = false, StatusCode = 500, Error = ex.Message }; // 500 generellt felmeddelande.
            }
        }
        
        return new ProductResult { Success = false, StatusCode = result.StatusCode, Error = result.Error, FieldErrors = result.FieldErrors};
    }

    public async Task<ProductResult> DeleteProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null) 
        {
            return new ProductResult { Success = false, StatusCode = 500, Error = "Product not found" };
        }

        try
        {
            _products = _products.Where(p => p.Id != product.Id).ToList();
            //_products.Remove(product);
            await _jsonFileRepository.WriteAsync(_products, cancellationToken);

            return new ProductResult { Success = true, StatusCode = 200 };
        }
        catch (Exception ex) 
        {
            return new ProductResult { Success = false, StatusCode=500, Error = ex.Message };
        }
    }
}
