using CXmlInvoiceGenerator.Application;
using CXmlInvoiceGenerator.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace CXmlInvoiceGeneratorTests.Application;

public class InvoiceLoaderTests
{
    [Fact]
    public async Task LoadNewInvoicesAsyncTest()
    {
        // Arrange

        var loggerMock = new Mock<ILogger<InvoiceLoader>>();
        var dbRepoMock = new Mock<IDatabaseRepository>();

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
        invoices.Rows.Add(new object[] { 1235, 5679, 54322, "GBP", 20, 4, 24, "T1", 20, 60 });

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

        dbRepoMock.Setup(m => m.GetNewInvoices()).Returns(invoices);
        dbRepoMock.Setup(m => m.GetItemsOnInvoice(It.IsAny<int>())).Returns(invoiceItems);
        dbRepoMock.Setup(m => m.GetBillingAddressForInvoice(It.IsAny<int>())).Returns(address.Rows[0]);
        dbRepoMock.Setup(m => m.GetDeliveryAddressForSalesOrder(It.IsAny<int>())).Returns(address.Rows[0]);

        var loader = new InvoiceLoader(loggerMock.Object, dbRepoMock.Object);

        // Act

        var results = await loader.LoadNewInvoicesAsync().ToListAsync();

        // Assert

        // First verify the database repo has been called to request the correct data
        dbRepoMock.Verify(m => m.GetItemsOnInvoice(1234), Times.Once);
        dbRepoMock.Verify(m => m.GetItemsOnInvoice(1235), Times.Once);
        dbRepoMock.Verify(m => m.GetBillingAddressForInvoice(1234), Times.Once);
        dbRepoMock.Verify(m => m.GetBillingAddressForInvoice(1235), Times.Once);
        dbRepoMock.Verify(m => m.GetDeliveryAddressForSalesOrder(54321), Times.Once);
        dbRepoMock.Verify(m => m.GetDeliveryAddressForSalesOrder(54322), Times.Once);

        // Now assert that a sample of the data returned by the loader is the same as the data supplied by the repo
        Assert.Equal(2, results.Count);
        Assert.Contains(results, i => i.Id == 1234);
        Assert.Contains(results, i => i.Id == 1235);
        var sampleInvoice = results.Single(i => i.Id == 1234);
        Assert.Equal(5678, sampleInvoice.CustomerId);
        Assert.Equal("GBP", sampleInvoice.CurrencyCode);
        Assert.Equal(12m, sampleInvoice.GrossAmount);
        Assert.Equal("Dean Dashwood", sampleInvoice.BillingAddress!.ContactName);
        Assert.Equal("Dean Dashwood", sampleInvoice.DeliveryAddress!.ContactName);
        Assert.Equal("Belkin", sampleInvoice.InvoiceItems!.Single().Manufacturer);
    }
}
