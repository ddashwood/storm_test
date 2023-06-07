namespace CXmlInvoiceGenerator.Services;

/// <summary>
/// Provides a set of date/time services. Putting these services in an interface instead
/// of accessing them directly from the .Net system classes enables them to be mocked
/// for easy unit testing.
/// </summary>
internal interface IDateTimeServices
{
    DateTime Now { get; }
}
