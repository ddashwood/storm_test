using CXmlInvoiceGenerator.Models;
using DatabaseAccess;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;

namespace CXmlInvoiceGenerator.Application;

internal class InvoiceGenerator : IInvoiceGenerator
{
    private readonly ILogger<InvoiceGenerator> _logger;

    public InvoiceGenerator(ILogger<InvoiceGenerator> logger)
    {
        _logger = logger;
    }

    public void GenerateCXMLForNewInvoices()
    {
        // == Please complete this function ==

        // 1) Using the DatabaseAccess dll provided and referenced (in the refs folder), load each invoice from the database
        //
        // 2) Create a cXml invoice document using the information from each invoice

        // The following is a very helpful resource for cXml:

        // https://compass.coupa.com/en-us/products/product-documentation/supplier-resources/supplier-integration-resources/standard-invoice-examples/sample-cxml-invoice-with-payment-terms

        // Assume the invoice is raised on the same day you find it, so PaymentTerms is from Today

        // VAT mode is header (overall total) only, not at item level

        // 3) Save the created invoices into a specified output file with the .xml file extension

        // The "purpose" for each invoice is "standard"
        // The "operation" for each invoice is "new"
        // The output folder is entirely up to you, based on your file system
        // You can use "fake" credentials (Domain/Identity/SharedSecret etc. etc.) of your own choosing for the From/To/Sender section for this test
        //
        // It would likely be a good idea for all of these to be configurable in some way, in a .Net options/settings file or an external ini file

        // Ideally, you will write reasonable progress steps to the console window

        // You may add references to anything you want from the standard Nuget URL

        // You may modify the signature to this function if you want to pass values into it

        // You may move this code into another class (or indeed classes) of your choosing

        DataTable newInvoices;
        try
        {
            Invoices invoiceDB = new();
            newInvoices = invoiceDB.GetNewInvoices();
            _logger.LogDebug("Number of invoices: " + newInvoices.Rows.Count);

            string invoiceNo = null!;

            foreach (DataRow invoiceRow in newInvoices.Rows)
            {
                try
                {
                    invoiceNo = "(Unknown)";
                    invoiceNo = invoiceRow["Id"].ToString() ?? invoiceNo;

                    var invoice = new Invoice(invoiceRow);
                    invoice.LoadInvoiceItems(invoiceDB.GetItemsOnInvoice(invoice.Id));
                    invoice.LoadBillingAddress(invoiceDB.GetBillingAddressForInvoice(invoice.Id));
                    invoice.LoadDeliveryAddress(invoiceDB.GetDeliveryAddressForSalesOrder(invoice.SalesOrderId));

                    _logger.LogDebug("Got row: " + invoice.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while working on invoice number " + invoiceNo);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error loading invoices");
            return;
        }


        // invoiceDB contains other functions you will need...
    }
}
