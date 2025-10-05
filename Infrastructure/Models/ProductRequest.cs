
namespace Infrastructure.Models;

public class ProductRequest
{
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductCategory { get; set; } = null!;    // Ska vara en ny klass
    public string ProductBrand { get; set; } = null!;        // Ska vara en ny klass
    public decimal ProductPrice { get; set; }
}