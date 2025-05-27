using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class AuditEntry
{
    public long Id { get; set; }
    public string EntityTypeCd { get; set; }
    public string EntityDescription { get; set; }
    public string EntityId { get; set; }
    public string EntityName { get; set; }
    public string Username { get; set; }
    public string ActionCd { get; set; }
    public string ActionDescription { get; set; }
    public string Note { get; set; }
    public DateTime Timestamp { get; set; }
}