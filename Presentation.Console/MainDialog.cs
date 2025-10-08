using Infrastructure.Models;
using Infrastructure.Services;

namespace Presentation.ConsoleApp;

internal class MainDialog(IProductService productService)
{

    private readonly IProductService _productService = productService;

    public void Show()
    {

    }

    private async Task Add()
    {
        var productRequest = new ProductRequest();

        Console.WriteLine("##### NEW PRODUCT #####");
    }
}
