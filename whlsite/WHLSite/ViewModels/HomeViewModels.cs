using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class HomeViewModel
{
    public QuoteViewModel Quote { get; set; }
    public VideoViewModel Video { get; set; }
}