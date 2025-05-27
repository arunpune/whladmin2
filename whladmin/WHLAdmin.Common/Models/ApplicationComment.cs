using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class ApplicationComment : ModelBase
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public long CommentId { get; set; }
    public bool InternalOnlyInd { get; set; }
    public string Comments { get; set; }
}