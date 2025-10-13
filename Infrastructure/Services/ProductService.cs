
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
                    Category = new Category { CategoryName = productRequest.Category.CategoryName },
                    Manufacturer = new Manufacturer { ManufacturerName = productRequest.Manufacturer.ManufacturerName },
                };

                // Check if the new name exists, Returns false if it exists
                var existing = ProductNotExistCheck(product);
                if (!existing.Success)
                {
                    return new ProductResult { Success = false, StatusCode = existing.StatusCode, Error = existing.Error, FieldErrors = existing.FieldErrors };
                }
                
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
        // Check if Edit for is valid
        var result = _inputValidationService.VerifyProductForm(product);

        if (result.Success)
        {
            try
            {
                var existingProduct = await GetProductByIdAsync(product.Id, cancellationToken);

                if (existingProduct.Content is not null)
                {
                    // If the new name is NOT equal the old name
                    if (!existingProduct.Content.ProductName.Equals(product.ProductName))
                    {
                        // Check if the new name exists, Returns false if it exists
                        var existing = ProductNotExistCheck(existingProduct.Content);
                        if (!existing.Success)
                        {
                            return new ProductResult { Success = false, StatusCode = existing.StatusCode, Error = existing.Error, FieldErrors = existing.FieldErrors };
                        }
                    }

                    existingProduct.Content.ProductName = product.ProductName;
                    existingProduct.Content.ProductDescription = product.ProductDescription;
                    existingProduct.Content.Category = product.Category;
                    existingProduct.Content.Manufacturer = product.Manufacturer;
                    existingProduct.Content.ProductPrice = product.ProductPrice;

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

    public InputResult ProductNotExistCheck(Product product)
    {
        var result = new InputResult();
        
        //if (_products.Any(p => p.Id == product.Id && p.ProductName != product.ProductName))
        if (_products.Any(p => p.Id == product.Id && p != product))
            result.FieldErrors.Add(new InputError { Field = "ID", Message = "Product ID already exists." });
        
        if (_products.Any(p => p.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
            result.FieldErrors.Add(new InputError { Field = "Name", Message = "Product name already exists." });  

        if (result.FieldErrors.Count > 0)
        {
            result.Success = false;
            result.StatusCode = 400;
            result.Error = "Fields have errors.";
        }
        else
        {
            result.Success = true;
            result.StatusCode = 204;
        }

        return result;
    }
}
