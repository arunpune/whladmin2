using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class LotteryViewModel : Lottery
{
    public string DisplayRunDate { get { return RunDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? RunDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
}

[ExcludeFromCodeCoverage]
public class LotteriesViewModel
{
    public IEnumerable<LotteryViewModel> Lotteries { get; set; }
}

[ExcludeFromCodeCoverage]
public class LotteryResultsViewModel : HousingApplicationsViewModel
{
    public int LotteryId { get; set; }
    public ListingViewModel ListingDetails { get; set; }
    public LotteryViewModel LotteryDetails { get; set; }
}
