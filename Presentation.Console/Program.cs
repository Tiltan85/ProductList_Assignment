using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => 
    {
        services.AddSingleton<IJsonFileRepository>(new JsonFileRepository("products.json"));
        services.AddSingleton<IProductService, ProductService>();
    })
    .Build();

