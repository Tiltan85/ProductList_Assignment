
namespace Infrastructure.Models;

public class ProductRequest
{
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public decimal ProductPrice { get; set; }
}