using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Repositories;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IReportsService
{
    DemographicsReportViewModel InitializeApplicationDemographicsReport();
    Task<string> GetApplicationDemographicsReport(DemographicsReportViewModel model);
    RegistrationsSummaryReportViewModel InitializeRegistrationsSummaryReport();
    Task<string> GetRegistrationsSummaryReport(RegistrationsSummaryReportViewModel model);
    Task<MarketEvaluationsReportViewModel> InitializeMarketEvaluationsReport();
    Task<string> GetMarketEvaluationsReport(MarketEvaluationsReportViewModel model);
}

public class ReportsService : IReportsService
{
    private readonly ILogger<ReportsService> _logger;
    private readonly IReportRepository _reportRepository;
    private readonly IListingsService _listingsService;
    private readonly IMetadataService _metadataService;

    public ReportsService(ILogger<ReportsService> logger, IReportRepository reportRepository,
                            IListingsService listingsService, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        _listingsService = listingsService ?? throw new ArgumentNullException(nameof(listingsService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    public DemographicsReportViewModel InitializeApplicationDemographicsReport()
    {
        var toDate = DateTime.Now.Day > 1 ? DateTime.Now : DateTime.Now.AddDays(-1);
        var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
        return new DemographicsReportViewModel()
        {
            FromDate = fromDate.ToString("yyyy-MM-dd"),
            ToDate = toDate.ToString("yyyy-MM-dd")
        };
    }

    public async Task<string> GetApplicationDemographicsReport(DemographicsReportViewModel model)
    {
        model.FromDate = (model.FromDate ?? "").Trim();
        model.ToDate = (model.ToDate ?? "").Trim();

        if (!DateTime.TryParse(model.FromDate, out var reportFromDate)
                || reportFromDate.Date < new DateTime(2024, 1, 1).Date
                || reportFromDate.Date > DateTime.Now.Date)
        {
            _logger.LogError($"From date must be on or after 1/1/2024, and on or before today");
            return "RP101";
        }

        DateTime reportToDate = DateTime.Now;
        if (!string.IsNullOrEmpty(model.ToDate))
        {
            if (!DateTime.TryParse(model.ToDate, out reportToDate)
                    || reportToDate.Date < reportFromDate.Date
                    || reportToDate.Date > DateTime.Now.Date)
            {
                _logger.LogError($"To date must be on or after From Date, and on or before today");
                return "RP102";
            }
        }

        var fromDate = int.Parse(reportFromDate.ToString("yyyyMMdd"));
        var toDate = int.Parse(reportToDate.ToString("yyyyMMdd"));

        model.GenderTypes = await _metadataService.GetGenderTypes();
        model.RaceTypes = await _metadataService.GetRaceTypes();
        model.EthnicityTypes = await _metadataService.GetEthnicityTypes();
        model.Data = [];

        var data = await _reportRepository.GetApplicationDemographicsReport(fromDate, toDate);
        if ((data?.Count() ?? 0) > 0)
        {
            var records = new List<ApplicationDemographicRecordViewModel>();
            var listingIds = data.Select(s => s.ListingId).Distinct();
            foreach (var listingId in listingIds)
            {
                var listingRecords = data.Where(w => w.ListingId == listingId);
                var listing = listingRecords.First();
                var record = new ApplicationDemographicRecordViewModel()
                {
                    ListingId = listingId,
                    ListingName = $"{listing.Name} ({listing.StreetLine1}, {listing.City}, {listing.StateCd} {listing.ZipCode})",
                    ListingTypeCd = listing.ListingTypeCd,
                    ListingTypeDescription = listing.ListingTypeDescription,
                    GenderData = [],
                    RaceData = [],
                    EthnicityData = []
                };
                foreach (var genderType in model.GenderTypes)
                {
                    record.GenderData.Add(genderType.Key, listingRecords.FirstOrDefault(w => w.GenderCd == genderType.Key)?.GenderCount ?? 0);
                }
                foreach (var raceType in model.RaceTypes)
                {
                    record.RaceData.Add(raceType.Key, listingRecords.FirstOrDefault(w => w.RaceCd == raceType.Key)?.RaceCount ?? 0);
                }
                foreach (var ethnicityType in model.EthnicityTypes)
                {
                    record.EthnicityData.Add(ethnicityType.Key, listingRecords.FirstOrDefault(w => w.EthnicityCd == ethnicityType.Key)?.EthnicityCount ?? 0);
                }
                records.Add(record);
            }
            model.Data = records;
        }
        model.Searched = true;
        return "";
    }

    public RegistrationsSummaryReportViewModel InitializeRegistrationsSummaryReport()
    {
        var toDate = DateTime.Now.Day > 1 ? DateTime.Now : DateTime.Now.AddDays(-1);
        var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
        return new RegistrationsSummaryReportViewModel()
        {
            FromDate = fromDate.ToString("yyyy-MM-dd"),
            ToDate = toDate.ToString("yyyy-MM-dd")
        };
    }

    public async Task<string> GetRegistrationsSummaryReport(RegistrationsSummaryReportViewModel model)
    {
        model.FromDate = (model.FromDate ?? "").Trim();
        model.ToDate = (model.ToDate ?? "").Trim();

        if (!DateTime.TryParse(model.FromDate, out var reportFromDate)
                || reportFromDate.Date < new DateTime(2024, 1, 1).Date
                || reportFromDate.Date > DateTime.Now.Date)
        {
            _logger.LogError($"From date must be on or after 1/1/2024, and on or before today");
            return "RP101";
        }

        DateTime reportToDate = DateTime.Now;
        if (!string.IsNullOrEmpty(model.ToDate))
        {
            if (!DateTime.TryParse(model.ToDate, out reportToDate)
                    || reportToDate.Date < reportFromDate.Date
                    || reportToDate.Date > DateTime.Now.Date)
            {
                _logger.LogError($"To date must be on or after From Date, and on or before today");
                return "RP102";
            }
        }

        var fromDate = int.Parse(reportFromDate.ToString("yyyyMMdd"));
        var toDate = int.Parse(reportToDate.ToString("yyyyMMdd"));

        model.NewYorkData = await _reportRepository.GetRegistrationsSummaryByCountyReport(fromDate, toDate, "NY");
        model.OtherStatesData = await _reportRepository.GetRegistrationsSummaryByStateReport(fromDate, toDate);
        model.TotalRegistrationsCount = (model.NewYorkData?.Sum(s => s.TotalCount) ?? 0) + (model.OtherStatesData?.Sum(s => s.TotalCount) ?? 0);
        model.TotalActiveCount = (model.NewYorkData?.Sum(s => s.ActiveCount) ?? 0) + (model.OtherStatesData?.Sum(s => s.ActiveCount) ?? 0);
        model.TotalInactiveCount = (model.NewYorkData?.Sum(s => s.InactiveCount) ?? 0) + (model.OtherStatesData?.Sum(s => s.InactiveCount) ?? 0);
        model.Searched = true;
        return "";
    }

    public async Task<MarketEvaluationsReportViewModel> InitializeMarketEvaluationsReport()
    {
        var toDate = DateTime.Now.Day > 1 ? DateTime.Now : DateTime.Now.AddDays(-1);
        var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
        return new MarketEvaluationsReportViewModel()
        {
            FromDate = fromDate.ToString("yyyy-MM-dd"),
            ToDate = toDate.ToString("yyyy-MM-dd"),
            ListingId = 0,
            Listings = await _listingsService.GetAll()
        };
    }

    public async Task<string> GetMarketEvaluationsReport(MarketEvaluationsReportViewModel model)
    {
        model.ListingId = model.ListingId < 0 ? 0 : model.ListingId;
        model.FromDate = (model.FromDate ?? "").Trim();
        model.ToDate = (model.ToDate ?? "").Trim();

        if (model.ListingId <= 0)
        {
            _logger.LogError($"Listing is required");
            return "RP103";
        }

        model.Listings = await _listingsService.GetAll();

        if (!DateTime.TryParse(model.FromDate, out var reportFromDate)
                || reportFromDate.Date < new DateTime(2024, 1, 1).Date
                || reportFromDate.Date > DateTime.Now.Date)
        {
            _logger.LogError($"From date must be on or after 1/1/2024, and on or before today");
            return "RP101";
        }

        DateTime reportToDate = DateTime.Now;
        if (!string.IsNullOrEmpty(model.ToDate))
        {
            if (!DateTime.TryParse(model.ToDate, out reportToDate)
                    || reportToDate.Date < reportFromDate.Date
                    || reportToDate.Date > DateTime.Now.Date)
            {
                _logger.LogError($"To date must be on or after From Date, and on or before today");
                return "RP102";
            }
        }

        var fromDate = int.Parse(reportFromDate.ToString("yyyyMMdd"));
        var toDate = int.Parse(reportToDate.ToString("yyyyMMdd"));

        model.Data = await _reportRepository.GetMarketEvaluationsReport(model.ListingId, fromDate, toDate);
        model.Searched = true;
        return "";
    }
}