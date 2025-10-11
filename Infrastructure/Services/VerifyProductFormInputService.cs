
using Infrastructure.Models;

namespace Infrastructure.Services;

public class VerifyProductFormInputService
{
    public async Task<ProductFormObjectResult<ProductFormInputResult>> VerifyProductFormInput(ProductRequest productRequest)
    {
        if (productRequest == null)
            return new ProductFormInputResult { Success = false, StatusCode = 400, Error = "Invalid Input" }; // 400 Bad Request, Fel input från användaren

        if (string.IsNullOrWhiteSpace(productRequest.ProductName))
            return new ProductResult { Success = false, StatusCode = 400, Error = "Product name can't be empty" };

        if (string.IsNullOrWhiteSpace(productRequest.ProductDescription))
            return new ProductResult { Success = false, StatusCode = 400, Error = "Product description can't be empty" };

        if (string.IsNullOrWhiteSpace(productRequest.Category))
            return new ProductResult { Success = false, StatusCode = 400, Error = "Product category can't be empty" };

        if (string.IsNullOrWhiteSpace(productRequest.Manufacturer))
            return new ProductResult { Success = false, StatusCode = 400, Error = "Product manufacturer can't be empty" };

        if (productRequest.ProductPrice < 0)
            return new ProductResult { Success = false, StatusCode = 400, Error = "Product price can't be negative" };

        return new ProductResult { Success = true, StatusCode = 204 };
    }
}
