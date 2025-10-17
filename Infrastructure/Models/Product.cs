
namespace Infrastructure.Models;

public class Product
{
    public string Id { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public Category Category { get; set; } = new();
    public Manufacturer Manufacturer { get; set; } = new();
    public decimal ProductPrice { get; set; }
}
