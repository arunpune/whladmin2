using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class PagedViewModel : PagingInfo
{
    public List<int> PageSizes { get; set; }
}