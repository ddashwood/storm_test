using CXmlInvoiceGenerator.Models;
using CXmlInvoiceGenerator.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CXmlInvoiceGenerator.Application;

internal class InvoiceLoader : IInvoiceLoader
{
    private readonly ILogger<InvoiceLoader> _logger;
    private readonly IDatabaseRepository _databaseRepository;

    public InvoiceLoader(ILogger<InvoiceLoader> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository;
    }

    public async IAsyncEnumerable<Invoice> LoadNewInvoicesAsync()
    {
        var newInvoices = _databaseRepository.GetNewInvoices();
        _logger.LogDebug("Number of invoices: " + newInvoices.Rows.Count);

        Invoice? invoice;
        string invoiceNo = null!;

        foreach (DataRow invoiceRow in newInvoices.Rows)
        {
            invoice = null;
            try
            {
                invoiceNo = "(Unknown)";
                invoiceNo = invoiceRow["Id"].ToString() ?? invoiceNo;

                invoice = new Invoice(invoiceRow);
                invoice.LoadInvoiceItems(_databaseRepository.GetItemsOnInvoice(invoice.Id));
                invoice.LoadBillingAddress(_databaseRepository.GetBillingAddressForInvoice(invoice.Id));
                invoice.LoadDeliveryAddress(_databaseRepository.GetDeliveryAddressForSalesOrder(invoice.SalesOrderId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while working on invoice number " + invoiceNo);
            }

            if (invoice != null)
            {
                // This method is async for future compatability - but does not currently need to await anything.
                // Here we use "await Task.FromResult(...)" purely to avoid warnings about not using await.
                yield return await Task.FromResult(invoice);
            }
        }
    }
}
