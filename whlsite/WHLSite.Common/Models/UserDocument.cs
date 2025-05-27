using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class UserDocument : ModelBase
{
    public string Username { get; set; }
    public long DocId { get; set; }
    public string DocTypeCd { get; set; }
    public string DocTypeDescription { get; set; }
    public string DocName { get; set; }
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public byte[] DocContents { get; set; }
}