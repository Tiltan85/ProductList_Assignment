
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Presentation.WPF.ViewModels;

public partial class ProductListViewModel : ObservableObject
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;

    public ProductListViewModel(IServiceProvider serviceProvider, IProductService productService)
    {
        _serviceProvider = serviceProvider;
        _productService = productService;

        LoadCommand = new AsyncRelayCommand(LoadProductListAsync);
        _ = LoadCommand.ExecuteAsync(null);

    }

    public IAsyncRelayCommand LoadCommand { get; }

    [ObservableProperty]
    private string _title = "Product List";

    [ObservableProperty]
    private ObservableCollection<Product> _productList = [];


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

    [RelayCommand]
    private void Edit(Product product)
    {
        var editViewModel = _serviceProvider.GetRequiredService<ProductEditViewModel>();
        editViewModel.SetProduct(product);
        
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = editViewModel;
    }

    [RelayCommand]
    private async Task Delete(Product product, CancellationToken cancellationToken = default)
    {
        
        System.Diagnostics.Debug.WriteLine($"DeleteProductAsync called. product = {(product == null ? "null" : product.Id)}");
        if (product == null) return;

        var ServiceResult = await _productService.DeleteProductAsync(product, cancellationToken);
        // TODO Error message if fail
        await LoadProductListAsync (cancellationToken);
    }

    [RelayCommand]
    private void NavigateToAddView()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<ProductAddViewModel>();
    }
}
