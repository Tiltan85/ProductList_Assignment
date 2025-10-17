
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
    private string? _error;

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
        var editResult = await _productService.EditProductAsync(ProductEditForm);
        if (editResult.Success)
        {
            // resets the form
            ProductEditForm = new();

            // navigates back to default view
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductDefaultViewModel>();

            // reloads the product list 
            var listViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
            await listViewModel.LoadCommand.ExecuteAsync(null);

            // updates the left view (list)
            mainViewModel.LeftViewModel = listViewModel;
        }
        Error = editResult.Error;
        FieldErrors = editResult.FieldErrors.ToDictionary(e => e.Field, e => e.Message);
    }

    [RelayCommand]
    private void Cancel()
    {
        // resets the form and error messages
        ProductEditForm = new();
        FieldErrors.Clear();

        // navigate right view back to default
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductDefaultViewModel>();
    }
}
