using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class Lottery : ModelBase
{
    public int LotteryId { get; set; }
    public long ListingId { get; set; }
    public bool ManualInd { get; set; }
    public DateTime? RunDate { get; set; }
    public string RunBy { get; set; }
    public string LotteryStatusCd { get; set; }
    public string LotteryStatusDescription { get; set; }
}