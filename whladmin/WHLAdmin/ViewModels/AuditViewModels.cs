using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class AuditViewModel : AuditEntry
{
}

[ExcludeFromCodeCoverage]
public class AuditViewerModel : AuditEntry
{
    public IEnumerable<AuditViewModel> Entries { get; set;}
}