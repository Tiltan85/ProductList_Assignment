using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.ConsoleApp;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => 
    {
        services.AddSingleton<IJsonFileRepository>(new JsonFileRepository("products.json"));
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<IInputValidationService, InputValidationService>();

        services.AddSingleton<MainDialog>();
    })
    .Build();

var main = host.Services.GetRequiredService<MainDialog>();
await main.Show();