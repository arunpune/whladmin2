using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class VideoConfig : ModelBase
{
    public int VideoId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }
    public int DisplayOrder { get; set; }
    public bool DisplayOnHomePageInd { get; set; }
}