using System.Data;

namespace CXmlInvoiceGenerator.Models;

/// <summary>
/// Represents an invoice.
/// </summary>
internal class Invoice
{
    public int Id { get; }
    public int CustomerId { get; }
    public int SalesOrderId { get; }
    public string CurrencyCode { get; }
    public decimal NetAmount { get; }
    public decimal VATAmount { get; }
    public decimal GrossAmount { get; }
    public string VATCode { get; }
    public decimal VATPercentage { get; }
    public int PaymentTermsDays { get; }

    private List<InvoiceItem>? _invoiceItems;
    public IReadOnlyList<InvoiceItem>? InvoiceItems => _invoiceItems?.AsReadOnly();

    public Address? BillingAddress { get; private set; }
    public Address? DeliveryAddress { get; private set; }

    public Invoice(DataRow row)
    {
        // The columns in the DataTable are all strings, so we need to convert them
        // This constructor will throw an exception if the conversion fails

        Id = Convert.ToInt32(row["Id"]);
        CustomerId = Convert.ToInt32(row["CustomerId"]);
        SalesOrderId = Convert.ToInt32(row["SalesOrderId"]);
        CurrencyCode = row["CurrencyCode"]?.ToString() ?? throw new NullReferenceException("Currency Code is null");
        NetAmount = Convert.ToDecimal(row["NetAmount"]);
        VATAmount = Convert.ToDecimal(row["VATAmount"]);
        GrossAmount = Convert.ToDecimal(row["GrossAmount"]);
        VATCode = row["VATCode"]?.ToString() ?? throw new NullReferenceException("VAT Code is null");
        VATPercentage = Convert.ToDecimal(row["VATPercentage"]);
        PaymentTermsDays = Convert.ToInt32(row["PaymentTermsDays"]);
    }

    public override string ToString()
    {
        return $"Invoice ID {Id} for customer {CustomerId} - currency {CurrencyCode}, gross amount {GrossAmount}";
    }

    public void LoadInvoiceItems(DataTable itemsTable)
    {
        _invoiceItems = new List<InvoiceItem>();

        foreach (DataRow row in itemsTable.Rows)
        {
            _invoiceItems.Add(new InvoiceItem(row));
        }
    }

    public void LoadBillingAddress(DataRow row)
    {
        BillingAddress = new Address(row);
    }

    public void LoadDeliveryAddress(DataRow row)
    {
        DeliveryAddress = new Address(row);
    }
}
