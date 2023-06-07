using System.Data;

namespace CXmlInvoiceGenerator.Models;

/// <summary>
/// Represents an address on an invoice.
/// </summary>
internal class Address
{
    public string ContactName { get; }
    public string AddrLine1 { get; }
    public string AddrLine2 { get; }
    public string AddrLine3 { get; }
    public string AddrLine4 { get; }
    public string AddrLine5 { get; }
    public string AddrPostCode { get; }
    public string CountryCode { get; }
    public string Country { get; }

    public Address(DataRow row)
    {
        ContactName = row["ContactName"]?.ToString() ?? "";
        AddrLine1 = row["AddrLine1"]?.ToString() ?? "";
        AddrLine2 = row["AddrLine2"]?.ToString() ?? "";
        AddrLine3 = row["AddrLine3"]?.ToString() ?? "";
        AddrLine4 = row["AddrLine4"]?.ToString() ?? "";
        AddrLine5 = row["AddrLine5"]?.ToString() ?? "";
        AddrPostCode = row["AddrPostCode"]?.ToString() ?? "";
        CountryCode = row["CountryCode"]?.ToString() ?? "";
        Country = row["Country"]?.ToString() ?? "";
    }
}
