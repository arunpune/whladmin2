using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class FaqConfig : ModelBase
{
    public int FaqId { get; set; }
    public string CategoryName { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }
    public string Url1 { get; set; }
    public string Url2 { get; set; }
    public string Url3 { get; set; }
    public string Url4 { get; set; }
    public string Url5 { get; set; }
    public string Url6 { get; set; }
    public string Url7 { get; set; }
    public string Url8 { get; set; }
    public string Url9 { get; set; }
    public int DisplayOrder { get; set; }
}