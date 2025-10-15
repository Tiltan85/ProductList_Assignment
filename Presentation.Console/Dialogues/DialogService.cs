
using Infrastructure.Models;
using Infrastructure.Services;

namespace Presentation.ConsoleApp.Dialogues;
public interface IDialogService
{
    Task AddProductDialog();
    Task ViewProductListDialog();
    Task ViewProductDialog(string productId);

}

public class DialogService(IProductService productService) : IDialogService
{

    private readonly IProductService _productService = productService;

    public async Task AddProductDialog()
    {
        bool loop = true;
        var productRequest = new ProductRequest();

        Console.Clear();
        Console.WriteLine("##### NEW PRODUCT #####");
        Console.WriteLine("");

        while (loop)
        {
            Console.Write("Enter product name: ");
            productRequest.ProductName = Console.ReadLine() ?? "";

            Console.Write("Enter product description: ");
            productRequest.ProductDescription = Console.ReadLine() ?? "";

            Console.Write("Enter product category: ");
            productRequest.Category.CategoryName = Console.ReadLine() ?? "";

            Console.Write("Enter product manufacturer: ");
            productRequest.Manufacturer.ManufacturerName = Console.ReadLine() ?? "";

            Console.Write("Enter product price: ");
            var price = Console.ReadLine();

            if (decimal.TryParse(price, out decimal convertedPrice))
            {
                productRequest.ProductPrice = convertedPrice;
            }

            var productResult = await _productService.SaveProductAsync(productRequest);

            if (!productResult.Success)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(productResult.Error);
                foreach (var item in productResult.FieldErrors)
                {
                    Console.WriteLine(item.Message);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey();

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Product Added");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey();
                loop = false;
            }

        }

        Console.Clear();
        return;
    }

    public async Task ViewProductListDialog()
    {
        Console.Clear();
        Console.WriteLine("############## PRODUCT LIST ##############");
        Console.WriteLine("");

        var productRequest = await _productService.GetProductAsync();
        List<ProductNumber> productNumber = [];
        int i = 0;

        if (productRequest.Content?.Count > 0)
        {
            Console.WriteLine("------------------------------------------");
            foreach (var product in productRequest.Content)
            {
                string name = product.ProductName.PadRight(20);
                string price = product.ProductPrice.ToString("0.00").PadLeft(6);

                // Skapa en lista för att välja produkt att titta på
                i++;
                productNumber.Add(new ProductNumber { Id = i, ProductId = product.Id });

                Console.WriteLine($"Press {i} to view product");
                Console.WriteLine($" Name: {name} Price: {price}");
                Console.WriteLine($" ID: {product.Id}");
                Console.WriteLine("------------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("No product found");
        }
        Console.WriteLine("Chose a number to view a product or press any other key to continue...");

        var productToView = Console.ReadLine();

        if (int.TryParse(productToView, out int convertedNumber))
        {
            var product = productNumber.FirstOrDefault(number => number.Id == convertedNumber);

            if (product != null)
                await ViewProductDialog(product.ProductId);
        }

        Console.Clear();
    }

    public async Task ViewProductDialog(string productId)
    {
        Console.Clear();
        if (string.IsNullOrWhiteSpace(productId))
        {
            Console.WriteLine("Something went wrong when selecting product");
            Console.ReadKey();
            return;
        }

        var result = await _productService.GetProductByIdAsync(productId);
        if (!result.Success)
        {
            Console.WriteLine(result.Error);
            Console.ReadKey();
            return;
        }
        
        var product = result.Content!;

        Console.WriteLine($"######## {product.ProductName.ToUpper()} ########");
        Console.WriteLine("Product ID:           " + product.Id);
        Console.WriteLine("Product Name:         " + product.ProductName);
        Console.WriteLine("Product Category:     " + product.Category?.CategoryName);
        Console.WriteLine("Product Description:  " + product.ProductDescription);
        Console.WriteLine("Product Manufacturer: " + product.Manufacturer?.ManufacturerName);
        Console.WriteLine("");

        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("1. Edit (Not Implemented)");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("2. Delete ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: PERMANENT!");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Press any other key to Contnue: ");

        string option = Console.ReadLine()!;

        switch (option)
        {
            case "2":
                var resultDelete = await _productService.DeleteProductAsync(product);
                if(resultDelete.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Product Removed");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                break;
        }

    }
}
