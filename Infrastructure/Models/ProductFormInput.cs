
namespace Infrastructure.Models;

public class ProductFormInput
{
    public string Id { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Manufacturer Manufacturer { get; set; } = null!;
    public decimal ProductPrice { get; set; }
}
