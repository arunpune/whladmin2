using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class DuplicateApplication
{
    public string DateOfBirth { get; set; }
    public string Last4SSN { get; set; }
    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string StreetAddress { get; set; }
    public int Count { get; set; }
}