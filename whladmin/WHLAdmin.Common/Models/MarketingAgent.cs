using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class MarketingAgent : ModelBase
{
    public int AgentId { get; set; }
    public string Name { get; set; }
    public string ContactName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public int UsageCount { get; set; }
    public bool Selected { get; set; }
}