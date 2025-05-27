using System;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class SystemInfoViewModel : SystemInfo
{
    public string DisplayTimestamp { get { return Timestamp.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? Timestamp.Value.ToString("yyyyMMddHHmmss") : ""; } }
}