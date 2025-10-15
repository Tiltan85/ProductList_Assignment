
namespace Infrastructure.Models;

public class ServiceObjectResult<T> : ServiceResult
{
    public T? Content { get; set; }
}
