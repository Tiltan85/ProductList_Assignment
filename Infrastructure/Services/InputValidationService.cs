
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Infrastructure.Services;

public class InputValidationService : IInputValidationService
{
    public ServiceResult VerifyProductForm(Product product)
    {
        var result = new ServiceResult();

        if (product == null)
        {
            result.Success = false;
            result.StatusCode = 400; // 400 Bad Request, Fel input från användaren
            result.Error = "Product edit form is empty";
            return result;
        }
        if (product.Id == null || string.IsNullOrWhiteSpace(product.Id))
            result.FieldErrors.Add(new InputError { Field = "Id", Message = "Product Id is missing." });

        if (string.IsNullOrWhiteSpace(product.ProductName))
            result.FieldErrors.Add(new InputError { Field = "Name", Message = "Product name can't be empty." });

        if (string.IsNullOrWhiteSpace(product.ProductDescription))
            result.FieldErrors.Add(new InputError { Field = "Description", Message = "Product description can't be empty." });

        if (product.Category == null || string.IsNullOrWhiteSpace(product.Category.CategoryName))
            result.FieldErrors.Add(new InputError { Field = "Category", Message = "Product category can't be empty." });

        if (product.Manufacturer == null || string.IsNullOrWhiteSpace(product.Manufacturer.ManufacturerName))
            result.FieldErrors.Add(new InputError { Field = "Manufacturer", Message = "Product manufacturer can't be empty." });

        if (product.ProductPrice < 0)
            result.FieldErrors.Add(new InputError { Field = "Price", Message = "Product pricec can't be negative value." });

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
    public ServiceResult VerifyProductForm(ProductRequest productRequest)
    {
        var result = new ServiceResult();

        if (productRequest == null)
        {
            result.Success = false;
            result.StatusCode = 400; // 400 Bad Request, Fel input från användaren
            result.Error = "Product edit form is empty";
            return result;
        }
        if (string.IsNullOrWhiteSpace(productRequest.ProductName))
            result.FieldErrors.Add(new InputError { Field = "Name", Message = "Product name can't be empty." });

        if (string.IsNullOrWhiteSpace(productRequest.ProductDescription))
            result.FieldErrors.Add(new InputError { Field = "Description", Message = "Product description can't be empty." });

        if (productRequest.Category == null || string.IsNullOrWhiteSpace(productRequest.Category.CategoryName))
            result.FieldErrors.Add(new InputError { Field = "Category", Message = "Product category can't be empty." });

        if (productRequest.Manufacturer == null || string.IsNullOrWhiteSpace(productRequest.Manufacturer.ManufacturerName))
            result.FieldErrors.Add(new InputError { Field = "Manufacturer", Message = "Product manufacturer can't be empty." });

        if (productRequest.ProductPrice < 0)
            result.FieldErrors.Add(new InputError { Field = "Price", Message = "Product pricec can't be negative value." });

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