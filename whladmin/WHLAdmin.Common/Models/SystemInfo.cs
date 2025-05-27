using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class SystemInfo
{
    public string ReleaseVersion { get; set; }
    public DateTime? Timestamp { get; set; }
}