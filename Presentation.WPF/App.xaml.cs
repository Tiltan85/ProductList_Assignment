using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.WPF.ViewModels;
using Presentation.WPF.Views;
using System.Windows;

namespace Presentation.WPF;

public partial class App : Application
{
    // Sätter upp en Host för att hantera Dependency Injection.
    private IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IJsonFileRepository>(new JsonFileRepository("products.json"));
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<IInputValidationService, InputValidationService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        services.AddTransient<ProductAddViewModel>();
        services.AddTransient<ProductAddView>();

        services.AddSingleton<ProductEditViewModel>();
        services.AddSingleton<ProductEditView>();

        services.AddTransient<ProductListViewModel>();
        services.AddTransient<ProductListView>();

    })
    .Build();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Sätter upp MainWindow och MainViewModel med Dependency Injection.
        var mainViewModel = host.Services.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = host.Services.GetRequiredService<ProductListViewModel>();

        
        var main = host.Services.GetRequiredService<MainWindow>();
        main.DataContext = mainViewModel;

        main.Show();
    }
}