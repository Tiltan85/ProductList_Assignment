using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Console;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => 
    {
        services.AddSingleton<IJsonFileRepository>(new JsonFileRepository("products.json"));
        services.AddSingleton<IProductService, ProductService>();

        services.AddSingleton<MainDialog>();
    })
    .Build();

var main = host.Services.GetRequiredService<MainDialog>();
main.Show();