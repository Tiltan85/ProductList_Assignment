
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
    private ProductRequest _productRequestForm = new();

    [RelayCommand]
    private async Task Save()
    {

        var productResult = await _productService.SaveProductAsync(ProductRequestForm);

        if (productResult.Success) 
        {
            ProductRequestForm = new();

            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        }
    }


    [RelayCommand]
    private void Cancel()
    {
        ProductRequestForm = new();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }

}
