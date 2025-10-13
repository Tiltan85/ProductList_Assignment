
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPF.ViewModels;

public partial class ProductEditViewModel(IServiceProvider serviceProvider, IProductService productService) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProductService _productService = productService;

    [ObservableProperty]
    private string _title = "Edit Product";

    [ObservableProperty]
    private Dictionary<string, string> _fieldErrors = [];

    [ObservableProperty]
    private Product _productEditForm = new();

    public void SetProduct(Product product)
    {
       ProductEditForm = new Product
        {
            Id = product.Id,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            Category = new Category { CategoryName = product.Category.CategoryName },
            Manufacturer = new Manufacturer { ManufacturerName = product.Manufacturer.ManufacturerName },
            ProductPrice = product.ProductPrice,
        };
    }

    [RelayCommand]
    private async Task Save()
    {
        var productResult = await _productService.EditProductAsync(ProductEditForm);
        if (productResult.Success)
        {
            ProductEditForm = new();

            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        }
        FieldErrors = productResult.FieldErrors.ToDictionary(e => e.Field, e => e.Message);
    }

    [RelayCommand]
    private void Cancel()
    {
        ProductEditForm = new();
        FieldErrors.Clear();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
    }
}
