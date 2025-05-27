using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Settings;

[ExcludeFromCodeCoverage]
public class EmfluenceSettings
{
    public bool Enabled { get; set; }
    public string ApiUrl { get; set; }
    public string ApiKey { get; set; }
    public string ApiKeyExpiry { get; set; }
    public string Domain { get; set; }
    public string RentalGroupId { get; set; }
    public string RentalGroupName { get; set; }
    public string RentalTemplateId { get; set; }
    public string RentalTemplateName { get; set; }
    public string SaleGroupId { get; set; }
    public string SaleGroupName { get; set; }
    public string SaleTemplateId { get; set; }
    public string SaleTemplateName { get; set; }
    public string CustomField1Name { get; set; }
    public string CustomField1Property { get; set; }
}
