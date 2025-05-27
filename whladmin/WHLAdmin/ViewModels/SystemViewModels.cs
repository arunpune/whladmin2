using System;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class SystemInfoViewModel : SystemInfo
{
    public string DisplayTimestamp { get { return Timestamp.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? Timestamp.Value.ToString("yyyyMMddHHmmss") : ""; } }
}