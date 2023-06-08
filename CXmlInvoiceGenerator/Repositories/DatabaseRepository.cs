using DatabaseAccess;
using System.Data;

namespace CXmlInvoiceGenerator.Repositories;

/// <inheritdoc cref="IDatabaseRepository"/>
internal class DatabaseRepository : IDatabaseRepository
{
    private readonly Invoices _invoiceDB;

    public DatabaseRepository(Invoices invoiceDB)
    {
        _invoiceDB = invoiceDB;
    }


    public DataTable GetNewInvoices() => _invoiceDB.GetNewInvoices();
    public DataTable GetItemsOnInvoice(int invoiceId) => _invoiceDB.GetItemsOnInvoice(invoiceId);
    public DataRow GetBillingAddressForInvoice(int invoiceId) => _invoiceDB.GetBillingAddressForInvoice(invoiceId);
    public DataRow GetDeliveryAddressForSalesOrder(int salesOrderId) => _invoiceDB.GetDeliveryAddressForSalesOrder(salesOrderId);
}
