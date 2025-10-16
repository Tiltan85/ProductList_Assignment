
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPF.ViewModels;

public partial class ProductAddViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    private string _title = "New Product";

    [ObservableProperty]
    private string? _error;

    [ObservableProperty]
    private Dictionary<string, string> _fieldErrors = [];

    [ObservableProperty]
    private ProductRequest _productRequestForm = new();

    [RelayCommand]
    private async Task Save()
    {
        var saveResult = await _productService.SaveProductAsync(ProductRequestForm);

        if (saveResult.Success) 
        {
            ProductRequestForm = new();

            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductDefaultViewModel>();

            var listViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
            await listViewModel.LoadCommand.ExecuteAsync(null);

            mainViewModel.LeftViewModel = listViewModel;
        }
        Error = saveResult.Error;
        FieldErrors = saveResult.FieldErrors.ToDictionary(e => e.Field, e => e.Message);
    }

    [RelayCommand]
    private void Cancel()
    {
        ProductRequestForm = new();
        FieldErrors.Clear();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductDefaultViewModel>();
    }

}
