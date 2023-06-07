using System.Data;

namespace CXmlInvoiceGenerator.Repositories;

internal interface IDatabaseRepository
{
    /// <summary>
    /// Gets new invoices from the database.
    /// </summary>
    /// <returns>A DataTable containing the invoices.</returns>
    DataTable GetNewInvoices();

    /// <summary>
    /// Gets items on an invoice from the database.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <returns>A DataTable containing the invoice lines.</returns>
    DataTable GetItemsOnInvoice(int invoiceId);

    /// <summary>
    /// Gets the delivery address for a sales order from the database.
    /// </summary>
    /// <param name="salesOrderId">The sales order ID.</param>
    /// <returns>A DataRow containing the address.</returns>
    DataRow GetDeliveryAddressForSalesOrder(int salesOrderId);

    /// <summary>
    /// Gets the billing address for an invoice from the database.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <returns>A DataRow containing the address.</returns>
    DataRow GetBillingAddressForInvoice(int invoiceId);
}
