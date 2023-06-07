using CXmlInvoiceGenerator.Application;
using CXmlInvoiceGenerator.Repositories;
using DatabaseAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IInvoiceGenerator, InvoiceGenerator>();
        services.AddScoped<IDatabaseRepository, DatabaseRepository>();
        services.AddScoped<IInvoiceLoader, InvoiceLoader>();
        services.AddSingleton(new Invoices());
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders()
            .AddConsole();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var program = scope.ServiceProvider.GetRequiredService<IInvoiceGenerator>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("New invoice generation run starting at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    await program.GenerateCXMLForNewInvoicesAsync();
    logger.LogInformation("New invoice generation run finishing at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
}