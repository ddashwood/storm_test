using CXmlInvoiceGenerator.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IInvoiceGenerator, InvoiceGenerator>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders()
            .AddConsole()
            .AddConfiguration(context.Configuration.GetSection("Logging"));
    })
    .Build();

var program = host.Services.GetRequiredService<IInvoiceGenerator>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("New invoice generation run starting at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
program.GenerateCXMLForNewInvoices();
logger.LogInformation("New invoice generation run finishing at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
