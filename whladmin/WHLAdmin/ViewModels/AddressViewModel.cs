using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class AddressViewModel
{
    public string StreetLine1 { get; set; }
    public string StreetLine2 { get; set; }
    public string StreetLine3 { get; set; }
    public string City { get; set; }
    public string StateCd { get; set; }
    public string ZipCode { get; set; }
    public string County { get; set; }
    public string CountyDescription { get; set; }
}