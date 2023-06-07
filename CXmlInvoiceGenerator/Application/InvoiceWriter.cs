using CXmlInvoiceGenerator.Configuration;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using static System.IO.Path;

namespace CXmlInvoiceGenerator.Application;

internal class InvoiceWriter : IInvoiceWriter
{
    private readonly WriterConfig _config;

    public InvoiceWriter(IOptions<WriterConfig> options)
    {
        _config = options.Value;
    }

    public Task WriteCxmlInvoice(cXML invoice)
    {
        var request = invoice.Items.OfType<Request>().Single();
        var detail = (InvoiceDetailRequest)request.Item;
        var id = detail.InvoiceDetailRequestHeader.invoiceID;
        Directory.CreateDirectory(_config.Path);
        var path = ChangeExtension(Combine(_config.Path, id), "xml");

        using (var stream = new StreamWriter(path))
        {
            // There are some properties which we want to include in generated XML even if they are the 
            // default. Here we override the default (set it to null) for these properties.

            XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();

            var attributes = new XmlAttributes()
            {
                XmlDefaultValue = null,
                XmlAttribute = new XmlAttributeAttribute()
            };

            attributeOverrides.Add(typeof(InvoiceDetailRequestHeader), "purpose", attributes);
            attributeOverrides.Add(typeof(InvoiceDetailRequestHeader), "operation", attributes);

            var serialiser = new XmlSerializer(typeof(cXML), attributeOverrides);

            // Now we can use the serializer with the attribute overrides set correctly.

            serialiser.Serialize(stream, invoice);
        }

        // There is currently no asynchronous API for serializing XML in the standard library.
        // See here for discussion about including it: https://github.com/dotnet/runtime/issues/32555
        // In the mean time, the method returns a Task for future compatability.
        return Task.CompletedTask;
    }
}
