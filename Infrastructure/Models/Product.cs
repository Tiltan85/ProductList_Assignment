
namespace Infrastructure.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Manufacturer Manufacturer { get; set; } = null!;
    public decimal ProductPrice { get; set; }
}
