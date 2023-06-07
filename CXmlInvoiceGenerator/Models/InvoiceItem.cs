using System.Data;

namespace CXmlInvoiceGenerator.Models;

internal class InvoiceItem
{
    public int Id { get; }
    public int InvoiceId { get; }
    public int StockItemId { get; }
    public string Manufacturer { get; }
    public string PartNo { get; }
    public string Description { get; }
    public int Qty { get; }
    public decimal UnitPrice { get; }
    public decimal LineTotal { get; }

    public InvoiceItem(DataRow row)
    {
        Id = Convert.ToInt32(row["Id"]);
        InvoiceId = Convert.ToInt32(row["InvoiceId"]);
        StockItemId = Convert.ToInt32(row["StockItemId"]);
        Manufacturer = row["Manufacturer"]?.ToString() ?? throw new NullReferenceException("Manufacturer is null");
        PartNo = row["PartNo"]?.ToString() ?? throw new NullReferenceException("Part No is null");
        Description = row["Description"]?.ToString() ?? throw new NullReferenceException("Description is null");
        Qty = Convert.ToInt32(row["Qty"]);
        UnitPrice = Convert.ToDecimal(row["UnitPrice"]);
        LineTotal = Convert.ToDecimal(row["LineTotal"]);
    }
}
