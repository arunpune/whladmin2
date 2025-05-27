using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class PagedRecords<T> where T : class
{
    public PagingInfo PagingInfo { get; set; }
    public IEnumerable<T> Records { get; set; }
}

[ExcludeFromCodeCoverage]
public class PagingInfo
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}