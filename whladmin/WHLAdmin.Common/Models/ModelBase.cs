using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public abstract class ModelBase
{
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool Active { get; set; }
}