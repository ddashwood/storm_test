using CXmlInvoiceGenerator.Application;
using CXmlInvoiceGenerator.Configuration;
using CXmlInvoiceGenerator.Repositories;
using CXmlInvoiceGenerator.Services;
using DatabaseAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IInvoiceGenerator, InvoiceGenerator>();
        services.AddScoped<IDatabaseRepository, DatabaseRepository>();
        services.AddScoped<IInvoiceLoader, InvoiceLoader>();
        services.AddScoped<ICxmlGenerator, CxmlGenerator>();
        services.AddScoped<IInvoiceWriter, InvoiceWriter>();
        services.AddScoped<IDateTimeServices, DateTimeServices>();
        services.AddSingleton(new Invoices());

        services.Configure<CxmlConfig>(context.Configuration.GetSection("CXML"));
        services.Configure<WriterConfig>(context.Configuration.GetSection("Writer"));
    })
    .ConfigureLogging(logging =>
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