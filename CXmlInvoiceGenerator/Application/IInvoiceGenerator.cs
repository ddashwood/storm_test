namespace CXmlInvoiceGenerator.Application;

internal interface IInvoiceGenerator
{
    /// <summary>
    /// Generates CXml for new invoices asynchronously.
    /// </summary>
    /// <returns>A Task representing the invoice generation.</returns>
    Task GenerateCXMLForNewInvoicesAsync();
}
