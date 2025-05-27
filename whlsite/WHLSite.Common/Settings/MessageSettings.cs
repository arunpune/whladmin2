using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Settings;

[ExcludeFromCodeCoverage]
public class MessageSettings
{
    public bool ShowDetailedExceptionMessages { get; set; }
    public Dictionary<string, string> Messages { get; set; }
}