
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
    private async Task SelectProduct(Product product)
    {
        if (product == null)
            return;

        var viewProductModel = _serviceProvider.GetRequiredService<ProductPresentationViewModel>();
        await viewProductModel.FindProductAsync(product);

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = viewProductModel;
    }

    [RelayCommand]
    private void NavigateToAddView()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RightViewModel = _serviceProvider.GetRequiredService<ProductAddViewModel>();
    }
}
