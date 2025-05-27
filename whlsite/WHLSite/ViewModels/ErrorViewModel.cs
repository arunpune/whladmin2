using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ErrorViewModel
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
    public string CorrelationId { get; set; }
    public string RequestId { get; set; }

    public bool ShowCorrelationId => !string.IsNullOrEmpty(CorrelationId);
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
