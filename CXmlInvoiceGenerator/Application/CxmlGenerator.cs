using CXmlInvoiceGenerator.Configuration;
using CXmlInvoiceGenerator.Models;
using CXmlInvoiceGenerator.Services;
using Microsoft.Extensions.Options;
using System.Xml;

namespace CXmlInvoiceGenerator.Application;

internal class CxmlGenerator : ICxmlGenerator
{
    private readonly CxmlConfig _config;
    private readonly IDateTimeServices _dateTimeServices;

    public CxmlGenerator(IOptions<CxmlConfig> options, IDateTimeServices dateTimeServices)
    {
        _config = options.Value;
        _dateTimeServices = dateTimeServices;
    }

    public cXML GenerateCxml(Invoice invoice)
    {
        if (invoice.InvoiceItems == null)
        {
            throw new NullReferenceException("Invoice must have its Invoice Items set");
        }
        if (invoice.BillingAddress == null)
        {
            throw new NullReferenceException("Invoice must have its Billing Address set");
        }
        if (invoice.DeliveryAddress == null)
        {
            throw new NullReferenceException("Invoice must have its Delivery Address set");
        }


        var result = new cXML();

        var header = BuildHeader();
        var request = new Request();
        var invoiceDetailRequest = GetInvoiceDetailRequest(invoice);
        var invoiceDetailOrder = GetInvoiceDetailOrder(invoice);

        invoiceDetailRequest.Items = new object[] { invoiceDetailOrder };
        invoiceDetailRequest.InvoiceDetailSummary = GetInvoiceDetailSummary(invoice);
        request.Item = invoiceDetailRequest;
        result.Items = new object[]
        {
            header,
            request
        };

        return result;
    }

    private static InvoiceDetailSummary GetInvoiceDetailSummary(Invoice invoice)
    {
        return new InvoiceDetailSummary
        {
            Tax = new Tax
            {
                TaxDetail = new TaxDetail[]
                {
                    new TaxDetail
                    {
                        TaxableAmount = new TaxableAmount { Money = new Money {currency = invoice.CurrencyCode, Value = invoice.NetAmount.ToString() }},
                        TaxAmount = new TaxAmount { Money = new Money {currency = invoice.CurrencyCode, Value = invoice.VATAmount.ToString() }},
                        percentageRate = invoice.VATPercentage.ToString(),
                        taxRateType = invoice.VATCode  // Is this the correct field to put this in? Need to clarify with product owner.
                    }
                }
            },
            GrossAmount = new GrossAmount { Money = new Money { currency = invoice.CurrencyCode, Value = invoice.GrossAmount.ToString() } },
            NetAmount = new NetAmount { Money = new Money { currency = invoice.CurrencyCode, Value = invoice.NetAmount.ToString() } },
            DueAmount = new DueAmount { Money = new Money { currency = invoice.CurrencyCode, Value = invoice.GrossAmount.ToString() } },
        };
    }

    private InvoiceDetailOrder GetInvoiceDetailOrder(Invoice invoice)
    {
        var invoiceDetailOrder = new InvoiceDetailOrder();
        invoiceDetailOrder.Items = invoice.InvoiceItems!.Select((item, i) => new InvoiceDetailItem
        {
            invoiceLineNumber = (i + 1).ToString(), // Adjust from 0-based to 1-based
            quantity = item.Qty.ToString(),
            UnitOfMeasure = "EA",
            UnitPrice = new UnitPrice { Money = new Money { currency = invoice.CurrencyCode, Value = item.UnitPrice.ToString() } },
            InvoiceDetailItemReference = new InvoiceDetailItemReference
            {
                ItemID = new ItemID { SupplierPartID = new SupplierPartID { Value = item.PartNo } },
                Description = new Description { lang = _config.ItemDescriptionLang, Text = new string[] { item.Description } },
                ManufacturerPartID = item.PartNo,
                ManufacturerName = new ManufacturerName { lang = _config.ManufacturerLang, Value = item.Manufacturer }
            },
            NetAmount = new NetAmount { Money = new Money { currency = invoice.CurrencyCode, Value = item.LineTotal.ToString() } },

            // To do - confirm with the product owner what type of rounding we should be using here, set to banker's rounding ("to even") for now.
            GrossAmount = new GrossAmount { Money = new Money { currency = invoice.CurrencyCode, Value = decimal.Round(item.LineTotal * (1 + invoice.VATPercentage / 100m), 2, MidpointRounding.ToEven).ToString() } },
        }).ToArray();
        return invoiceDetailOrder;
    }

    private InvoiceDetailRequest GetInvoiceDetailRequest(Invoice invoice)
    {
        return new InvoiceDetailRequest
        {
            InvoiceDetailRequestHeader = new InvoiceDetailRequestHeader()
            {
                invoiceID = invoice.Id.ToString(),
                purpose = Enum.Parse<InvoiceDetailRequestHeaderPurpose>(_config.Purpose),
                operation = Enum.Parse<InvoiceDetailRequestHeaderOperation>(_config.Operation),
                invoiceDate = _dateTimeServices.Now.ToString("s"),
                InvoicePartner = new InvoicePartner[]
                {
                    GetInvoicePartnerFromAddress(invoice.DeliveryAddress!, "soldTo"),
                    GetInvoicePartnerFromAddress(invoice.BillingAddress!, "billTo")
                },
                Items = new object[]
                {
                    new PaymentTerm
                    {
                        payInNumberOfDays = invoice.PaymentTermsDays.ToString()
                    }
                }
            },

        };
    }

    private InvoicePartner GetInvoicePartnerFromAddress(Models.Address address, string role)
    {
        return new InvoicePartner
        {
            Contact = new Contact
            {
                role = role,
                Name = new Name { lang = _config.PartnerNameLang, Value = address.ContactName },
                PostalAddress = new PostalAddress[]
                {
                    new PostalAddress
                    {
                        Street = new string[]
                        {
                            address.AddrLine1,
                            address.AddrLine2,
                            address.AddrLine3,
                            address.AddrLine4,
                            address.AddrLine5
                        },
                        PostalCode = address.AddrPostCode,
                        Country = new Country { isoCountryCode = address.CountryCode, Value = address.Country }
                    }
                }
            }
        };
    }

    private Header BuildHeader()
    {
        var header = new Header();

        header.From = new From();
        var fromIdentityNode = new XmlDocument().CreateNode(XmlNodeType.Text, "Identity", "");
        fromIdentityNode.InnerText = _config.FromCredentialIdentity;
        header.From.Credential = new Credential[]
        {
            new Credential
            {
                domain = _config.FromCredentialDomain,
                Identity = new Identity { Any = new XmlNode[] { fromIdentityNode  } }
            }
        };

        header.To = new To();
        var toIdentityNode = new XmlDocument().CreateNode(XmlNodeType.Text, "Identity", "");
        toIdentityNode.InnerText = _config.ToCredentialIdentity;
        header.To.Credential = new Credential[]
        {
            new Credential
            {
                domain = _config.ToCredentialDomain,
                Identity = new Identity { Any = new XmlNode[] { toIdentityNode  } }
            }
        };

        header.Sender = new Sender();
        var senderIdentityNode = new XmlDocument().CreateNode(XmlNodeType.Text, "Identity", "");
        senderIdentityNode.InnerText = _config.SenderCredentialIdentity;
        header.Sender.Credential = new Credential[]
        {
            new Credential
            {
                domain = _config.SenderCredentialDomain,
                Identity = new Identity { Any = new XmlNode[] { senderIdentityNode  } }
            }
        };

        return header;
    }
}
