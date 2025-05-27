using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class VideoViewModel : VideoConfig
{
}

[ExcludeFromCodeCoverage]
public class VideosViewModel
{
    public IEnumerable<VideoViewModel> Videos { get; set; }
}