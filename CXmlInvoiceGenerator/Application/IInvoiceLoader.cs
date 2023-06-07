using CXmlInvoiceGenerator.Models;

namespace CXmlInvoiceGenerator.Application;

internal interface IInvoiceLoader
{
    /// <summary>
    /// Loads new invoices from the database asynchronously. Note that the current
    /// implementation of this interface works synchronously, but the interface
    /// supports future asynchronous implementations.
    /// </summary>
    /// <returns>An IAsyncEnumerable containing the invoices.</returns>
    IAsyncEnumerable<Invoice> LoadNewInvoices();
}
