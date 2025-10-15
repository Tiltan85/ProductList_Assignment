using Infrastructure.Models;
using Infrastructure.Services;

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
            Console.WriteLine("3. Exit Application");
            Console.WriteLine("");
            Console.Write("Select menu option: ");
            var option = Console.ReadLine();

            switch (option) 
            {
                case "1":                    
                    await ViewProductListDialog();
                    break;

                case "2":
                    await AddProductDialog(); 
                    break;

                case "3":
                    Environment.Exit(0); 
                    break;
            }
        }
    }
}
