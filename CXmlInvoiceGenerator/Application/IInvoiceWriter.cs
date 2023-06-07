namespace CXmlInvoiceGenerator.Application;

internal interface IInvoiceWriter
{
    /// <summary>
    /// Writes a CXml invoice asynchronously to the file system asynchronously.
    /// </summary>
    /// <param name="invoice">The CXml invoice to write.</param>
    /// <returns>A Task representing the writing of the file.</returns>
    Task WriteCxmlInvoiceAsync(cXML invoice);
}
