
namespace Infrastructure.Models
{

    public class ProductFormObjectResult<T> : ProductFormInputResult
    {
        public T? Content { get; set; }
    }
}
