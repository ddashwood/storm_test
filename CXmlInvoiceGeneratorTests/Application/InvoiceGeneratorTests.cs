using CXmlInvoiceGenerator.Application;
using CXmlInvoiceGenerator.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace CXmlInvoiceGeneratorTests.Application;

public class InvoiceGeneratorTests
{
    [Fact]
    public async Task GenerateCXMLForNewInvoicesAsyncTest()
    {
        // Arrange

        var loggerMock = new Mock<ILogger<InvoiceGenerator>>();
        var loaderMock = new Mock<IInvoiceLoader>();
        var generatorMock = new Mock<ICxmlGenerator>();
        var writerMock = new Mock<IInvoiceWriter>();

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

        generatorMock.SetupSequence(m => m.GenerateCxml(It.IsAny<Invoice>()))
            .Returns(new cXML { payloadID = "Test1" })
            .Returns(new cXML { payloadID = "Test2" });

        var data = new List<Invoice>
        {
            new Invoice(invoices.Rows[0]),
            new Invoice(invoices.Rows[1])
        }.ToAsyncEnumerable();

        loaderMock.Setup(m => m.LoadNewInvoices()).Returns(data);

        var generator = new InvoiceGenerator(loggerMock.Object, loaderMock.Object, generatorMock.Object, writerMock.Object);

        // Act

        await generator.GenerateCXMLForNewInvoicesAsync();

        // Assert

        generatorMock.Verify(m => m.GenerateCxml(It.Is<Invoice>(i => i.Id == 1234)), Times.Once);
        generatorMock.Verify(m => m.GenerateCxml(It.Is<Invoice>(i => i.Id == 1235)), Times.Once);
        writerMock.Verify(m => m.WriteCxmlInvoice(It.Is<cXML>(cxml => cxml.payloadID == "Test1")), Times.Once);
        writerMock.Verify(m => m.WriteCxmlInvoice(It.Is<cXML>(cxml => cxml.payloadID == "Test2")), Times.Once);
    }
}
