using CXmlInvoiceGenerator.Models;

namespace CXmlInvoiceGenerator.Application;

internal interface ICxmlGenerator
{
    /// <summary>
    /// Generate CXml for an invoice.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns>A CXml object representing the invoice.</returns>
    cXML GenerateCxml(Invoice invoice);
}
