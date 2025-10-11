using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IInputValidationService
    {
        InputResult VerifyProductForm(Product product);
    }
}