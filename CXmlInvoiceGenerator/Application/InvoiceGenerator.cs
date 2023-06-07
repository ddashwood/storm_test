using Microsoft.Extensions.Logging;
using System;
using System.Xml.Serialization;

namespace CXmlInvoiceGenerator.Application;

internal class InvoiceGenerator : IInvoiceGenerator
{
    private readonly ILogger<InvoiceGenerator> _logger;
    private readonly IInvoiceLoader _invoiceLoader;
    private readonly ICxmlGenerator _cxmlGenerator;
    private readonly IInvoiceWriter _invoiceWriter;

    public InvoiceGenerator(ILogger<InvoiceGenerator> logger, IInvoiceLoader invoiceLoader,
            ICxmlGenerator cxmlGenerator, IInvoiceWriter invoiceWriter)
    {
        _logger = logger;
        _invoiceLoader = invoiceLoader;
        _cxmlGenerator = cxmlGenerator;
        _invoiceWriter = invoiceWriter;
    }

    public async Task GenerateCXMLForNewInvoicesAsync()
    {
        try
        {
            await foreach (var invoice in _invoiceLoader.LoadNewInvoices())
            {
                _logger.LogDebug("Got row: " + invoice.ToString());
                var cxml = _cxmlGenerator.GenerateCxml(invoice);
                _logger.LogDebug("Converted row to CXml");
                await _invoiceWriter.WriteCxmlInvoice(cxml);
                _logger.LogDebug("CXml written to file system");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error loading invoices");
            return;
        }
    }
}
