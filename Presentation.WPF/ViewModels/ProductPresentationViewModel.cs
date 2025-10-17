
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WPF.ViewModels;

public partial class ProductPresentationViewModel : ObservableObject
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;

    public ProductPresentationViewModel(IServiceProvider serviceProvider, IProductService productService)
    {
        _serviceProvider = serviceProvider;
        _productService = productService;

        LoadCommand = new AsyncRelayCommand(LoadProductListAsync);
        _ = LoadCommand.ExecuteAsync(null);

    }

    public IAsyncRelayCommand LoadCommand { get; }

    [ObservableProperty]
    private string _title = "Product Information";

    [ObservableProperty]
    private string? _error;

    [ObservableProperty]
    private ObservableCollection<Product> _productList = [];

    [ObservableProperty]
    private Product _selectedProduct = new();

    private async Task LoadProductListAsync(CancellationToken cancellationToken = default)
    {
        var ServiceResult = await _productService.GetProductAsync(cancellationToken);

        if (ServiceResult.Content != null)
        {   // Sort list by name.
            var sortedList = ServiceResult.Content
                .OrderBy(p => p.ProductName)
                .ToList();

            ProductList = new ObservableCollection<Product>(sortedList);
        }
    }
    public async Task FindProductAsync(Product product)
    {
        if (product == null)
            return;

        var result = await _productService.GetProductByIdAsync(product.Id);

        if (result.Success && result.Content != null)
        {
            SelectedProduct = result.Content;
        }
    }

    [RelayCommand]
    private void Edit(Product product)
    {
        var editViewModel = _serviceProvider.GetRequiredService<ProductEditViewModel>();
        editViewModel.SetProduct(product);

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = editViewModel;
    }

    [RelayCommand]
    private async Task Delete(Product product, CancellationToken cancellationToken = default)
    {
        // safety check returns if product is null
        if (product == null) return;

        // calls the service to delete the product
        var ServiceResult = await _productService.DeleteProductAsync(product, cancellationToken);
        if (!ServiceResult.Success)
        {
            Error = ServiceResult.Error ?? "Failed to remove product";
            return;
        }

        // navigates right view back to default view
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductDefaultViewModel>();

        // reloads the product list 
        var listViewModel = _serviceProvider.GetRequiredService<ProductListViewModel>();
        await listViewModel.LoadCommand.ExecuteAsync(null);

        // updates the left view (list)
        mainViewModel.LeftViewModel = listViewModel;
    }
}
