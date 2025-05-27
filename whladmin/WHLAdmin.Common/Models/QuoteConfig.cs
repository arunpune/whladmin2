using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class QuoteConfig : ModelBase
{
    public int QuoteId { get; set; }
    public string Text { get; set; }
    public bool DisplayOnHomePageInd { get; set; }
}