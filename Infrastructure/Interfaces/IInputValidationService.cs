using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IInputValidationService
    {
        ServiceResult VerifyProductForm(Product product);
        ServiceResult VerifyProductForm(ProductRequest productRequest);
    }
}