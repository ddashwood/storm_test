using Microsoft.Extensions.Logging;
using System;
using System.Xml.Serialization;

namespace CXmlInvoiceGenerator.Application;

internal class InvoiceGenerator : IInvoiceGenerator
{
    private readonly ILogger<InvoiceGenerator> _logger;
    private readonly IInvoiceLoader _invoiceLoader;
    private readonly ICxmlGenerator _cxmlGenerator;

    public InvoiceGenerator(ILogger<InvoiceGenerator> logger, IInvoiceLoader invoiceLoader, ICxmlGenerator cxmlGenerator)
    {
        _logger = logger;
        _invoiceLoader = invoiceLoader;
        _cxmlGenerator = cxmlGenerator;
    }

    public async Task GenerateCXMLForNewInvoicesAsync()
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

        try
        {
            await foreach (var invoice in _invoiceLoader.LoadNewInvoices())
            {
                _logger.LogDebug("Got row: " + invoice.ToString());
                var cxml = _cxmlGenerator.GenerateCxml(invoice);

                using (var stream = new MemoryStream())
                {
                    // There are some properties which we want to include in generated XML even if they are the 
                    // default. Here we override the default (set it to null) for these properties.

                    XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();

                    var attributes = new XmlAttributes()
                    {
                        XmlDefaultValue = null,
                        XmlAttribute = new XmlAttributeAttribute()
                    };

                    attributeOverrides.Add(typeof(InvoiceDetailRequestHeader), "purpose", attributes);
                    attributeOverrides.Add(typeof(InvoiceDetailRequestHeader), "operation", attributes);

                    var serialiser = new XmlSerializer(typeof(cXML), attributeOverrides);

                    // Now we can use the serializer with the attribute overrides set correctly.

                    serialiser.Serialize(stream, cxml);

                    stream.Position = 0;

                    using(var reader = new StreamReader(stream))
                    {
                        var xml = await reader.ReadToEndAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error loading invoices");
            return;
        }
    }
}
