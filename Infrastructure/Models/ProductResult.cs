
namespace Infrastructure.Models;

public class ProductResult : InputResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }
    public Product? Content { get; set; }
}
