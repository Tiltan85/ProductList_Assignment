
namespace Infrastructure.Models;

internal class InputObjectResult<T> : InputResult
{
    public T? Content { get; set; }
}
