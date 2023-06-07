using CXmlInvoiceGenerator.Application;
using CXmlInvoiceGenerator.Configuration;
using CXmlInvoiceGenerator.Models;
using CXmlInvoiceGenerator.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Data;

namespace CXmlInvoiceGeneratorTests.Application;

public class CxmlGeneratorTests
{
    [Fact]
    public void NoItemsTest()
    {
        // Arrange

        var invoice = GetInvoice();
        AddBillingAddress(invoice);
        AddDeliveryAddress(invoice);

        var optionsMock = new Mock<IOptions<CxmlConfig>>();
        var dateTimeMock = new Mock<IDateTimeServices>();

        var generator = new CxmlGenerator(optionsMock.Object, dateTimeMock.Object);

        // Act and Assert

        var ex = Assert.Throws<NullReferenceException>(() => generator.GenerateCxml(invoice));
        Assert.Equal("Invoice must have its Invoice Items set", ex.Message);
    }

    [Fact]
    public void NoBillingAddressTest()
    {
        // Arrange

        var invoice = GetInvoice();
        AddItems(invoice);
        AddDeliveryAddress(invoice);

        var optionsMock = new Mock<IOptions<CxmlConfig>>();
        var dateTimeMock = new Mock<IDateTimeServices>();

        var generator = new CxmlGenerator(optionsMock.Object, dateTimeMock.Object);

        // Act and Assert

        var ex = Assert.Throws<NullReferenceException>(() => generator.GenerateCxml(invoice));
        Assert.Equal("Invoice must have its Billing Address set", ex.Message);
    }

    [Fact]
    public void NoDeliveryAddressTest()
    {
        // Arrange

        var invoice = GetInvoice();
        AddItems(invoice);
        AddBillingAddress(invoice);

        var optionsMock = new Mock<IOptions<CxmlConfig>>();
        var dateTimeMock = new Mock<IDateTimeServices>();

        var generator = new CxmlGenerator(optionsMock.Object, dateTimeMock.Object);

        // Act and Assert

        var ex = Assert.Throws<NullReferenceException>(() => generator.GenerateCxml(invoice));
        Assert.Equal("Invoice must have its Delivery Address set", ex.Message);
    }

    [Fact]
    public void GenerateCxmlText()
    {
        // Arrange

        var invoice = GetInvoice();
        AddItems(invoice);
        AddBillingAddress(invoice);
        AddDeliveryAddress(invoice);

        var optionsMock = new Mock<IOptions<CxmlConfig>>();
        var dateTimeMock = new Mock<IDateTimeServices>();

        optionsMock.Setup(m => m.Value).Returns(new CxmlConfig { Purpose = "standard", Operation = "new" });

        var generator = new CxmlGenerator(optionsMock.Object, dateTimeMock.Object);

        // Act

        var cxml = generator.GenerateCxml(invoice);

        // Assert

        // Check a sample of the data in the CXml - other data checks can be added as required later
        var request = cxml.Items.OfType<Request>().Single();
        var detail = (InvoiceDetailRequest)request.Item;
        var id = detail.InvoiceDetailRequestHeader.invoiceID;
        Assert.Equal("1234", id);
    }

    private static Invoice GetInvoice()
    {
        var invoices = new DataTable();
        invoices.Columns.Add("Id");
        invoices.Columns.Add("CustomerId");
        invoices.Columns.Add("SalesOrderId");
        invoices.Columns.Add("CurrencyCode");
        invoices.Columns.Add("NetAmount");
        invoices.Columns.Add("VATAmount");
        invoices.Columns.Add("GrossAmount");
        invoices.Columns.Add("VATCode");
        invoices.Columns.Add("VATPercentage");
        invoices.Columns.Add("PaymentTermsDays");
        invoices.Rows.Add(new object[] { 1234, 5678, 54321, "GBP", 10, 2, 12, "T1", 20, 30 });

        return new Invoice(invoices.Rows[0]);
    }

    private void AddItems(Invoice invoice)
    {
        DataTable invoiceItems = new DataTable();
        invoiceItems.Columns.Add("Id");
        invoiceItems.Columns.Add("InvoiceId");
        invoiceItems.Columns.Add("StockItemId");
        invoiceItems.Columns.Add("Manufacturer");
        invoiceItems.Columns.Add("PartNo");
        invoiceItems.Columns.Add("Description");
        invoiceItems.Columns.Add("Qty");
        invoiceItems.Columns.Add("UnitPrice");
        invoiceItems.Columns.Add("LineTotal");
        invoiceItems.Rows.Add(new object[] { 654, 321, 98765, "Belkin", "Belk654", "Cable", 2, 20m, 40m });

        invoice.LoadInvoiceItems(invoiceItems);
    }

    private void AddBillingAddress(Invoice invoice)
    {
        DataTable address = new DataTable();
        address.Columns.Add("ContactName");
        address.Columns.Add("AddrLine1");
        address.Columns.Add("AddrLine2");
        address.Columns.Add("AddrLine3");
        address.Columns.Add("AddrLine4");
        address.Columns.Add("AddrLine5");
        address.Columns.Add("AddrPostCode");
        address.Columns.Add("CountryCode");
        address.Columns.Add("Country");
        address.Rows.Add(new object[] { "Dean Dashwood", "1 Somewhere St", "Wembley", "", "", "London", "HA9 7AA", "GB", "Great Britain" });

        invoice.LoadBillingAddress(address.Rows[0]);
    }

    private void AddDeliveryAddress(Invoice invoice)
    {
        DataTable address = new DataTable();
        address.Columns.Add("ContactName");
        address.Columns.Add("AddrLine1");
        address.Columns.Add("AddrLine2");
        address.Columns.Add("AddrLine3");
        address.Columns.Add("AddrLine4");
        address.Columns.Add("AddrLine5");
        address.Columns.Add("AddrPostCode");
        address.Columns.Add("CountryCode");
        address.Columns.Add("Country");
        address.Rows.Add(new object[] { "Dean Dashwood", "1 Somewhere St", "Wembley", "", "", "London", "HA9 7AA", "GB", "Great Britain" });

        invoice.LoadDeliveryAddress(address.Rows[0]);
    }
}
