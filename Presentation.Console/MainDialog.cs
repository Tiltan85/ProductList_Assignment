using Infrastructure.Models;
using Infrastructure.Services;
using System.Threading.Tasks;

namespace Presentation.ConsoleApp;

internal class MainDialog(IProductService productService)
{

    private readonly IProductService _productService = productService;

    public async Task Show()
    {
        while (true)
        {
            
            Console.WriteLine("##### MENU OPTIONS #####");
            Console.WriteLine("");
            Console.WriteLine("1. View Product List");
            Console.WriteLine("2. Add New Product");
            Console.WriteLine("");
            Console.Write("Select menu option: ");
            var option = Console.ReadLine();

            switch (option) 
            {
                case "1":                    
                    await View();
                    break;

                case "2":
                    await Add(); 
                    break;
            }

        }
    }

    private async Task Add()
    {
        var productRequest = new ProductRequest();

        Console.WriteLine("##### NEW PRODUCT #####");
        Console.WriteLine("");

        Console.Write("Enter product name: ");
        productRequest.ProductName = Console.ReadLine() ?? "";

        Console.Write("Enter product price: ");
        var price = Console.ReadLine();

        if (decimal.TryParse(price, out decimal convertedPrice))
        {
            productRequest.ProductPrice = convertedPrice;
        }

        var productResult = await _productService.SaveProductAsync(productRequest);
        // if (productResult.Success) return;
        Console.Clear();
    }

    private async Task View()
    {
        Console.Clear();
        Console.WriteLine("############## PRODUCT LIST ##############");
        Console.WriteLine("");

        var productRequest = await _productService.GetProductAsync();

        if (productRequest.Content?.Count > 0)
        {
            Console.WriteLine("------------------------------------------");
            foreach (var product in productRequest.Content)
            {
                string name = product.ProductName.PadRight(20);
                string price = product.ProductPrice.ToString("0.00").PadLeft(6);
                
                Console.WriteLine($" Name: {name} Price: {price}");
                Console.WriteLine($" ID: {product.Id}");
                Console.WriteLine("------------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("No product found");
        }

        Console.ReadKey();
        Console.Clear();
    }
}
