using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Presentation.WPF;

public partial class App : Application
{
    private IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IJsonFileRepository>(new JsonFileRepository("products.json"));
        services.AddSingleton<IProductService, ProductService>();

        services.AddSingleton<MainWindow>();
    })
    .Build();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var main = host.Services.GetRequiredService<MainWindow>();
    }
}