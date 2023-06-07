namespace CXmlInvoiceGenerator.Configuration;

internal class CxmlConfig
{
    public string FromCredentialDomain { get; set; } = "";
    public string FromCredentialIdentity { get; set; } = "";
    public string ToCredentialDomain { get; set; } = "";
    public string ToCredentialIdentity { get; set; } = "";
    public string SenderCredentialDomain { get; set; } = "";
    public string SenderCredentialIdentity { get; set; } = "";

    public string Purpose { get; set; } = "";
    public string Operation { get; set; } = "";

    public string PartnerNameLang { get; set; } = "";
    public string ItemDescriptionLang { get; set; } = "";
    public string ManufacturerLang { get; set; } = "";
}
