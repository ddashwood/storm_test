using CXmlInvoiceGenerator.Models;

namespace CXmlInvoiceGenerator.Application;

internal interface ICxmlGenerator
{
    cXML GenerateCxml(Invoice invoice);
}
