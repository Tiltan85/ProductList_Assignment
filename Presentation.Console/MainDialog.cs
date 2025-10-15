using Infrastructure.Models;
using Infrastructure.Services;
using Presentation.ConsoleApp.Dialogues;

namespace Presentation.ConsoleApp;

internal class MainDialog(IDialogService dialogService)
{

    private readonly IDialogService _dialogService = dialogService;

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
                    await _dialogService.ViewProductListDialog();
                    break;

                case "2":
                    await _dialogService.AddProductDialog(); 
                    break;

                case "3":
                    Environment.Exit(0); 
                    break;
            }
        }
    }
}
