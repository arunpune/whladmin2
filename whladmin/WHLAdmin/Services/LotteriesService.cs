using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface ILotteriesService
{
    Task<ListingsViewModel> GetEligibleListings(string requestId, string correlationId, string userId);
    Task<string> RunLottery(string requestId, string correlationId, string userId, long listingId, bool rerun = false);
    Task<LotteryResultsViewModel> GetResults(string requestId, string correlationId, string userId, int lotteryId, int pageNo = 1, int pageSize = 1000);
    Task<string> GetDownloadData(string requestId, string correlationId, string username, int lotteryId);
}

public class LotteriesService : ILotteriesService
{
    private readonly ILogger<LotteriesService> _logger;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IListingsService _listingsService;
    private readonly IUsersService _usersService;

    public LotteriesService(ILogger<LotteriesService> logger, ILotteryRepository lotteryRepository
                                , IListingsService listingsService, IMetadataService metadataService
                                , IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _lotteryRepository = lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
        _listingsService = listingsService ?? throw new ArgumentNullException(nameof(listingsService));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<ListingsViewModel> GetEligibleListings(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var listings = await _lotteryRepository.GetEligibleListings();
        return new ListingsViewModel()
        {
            Listings = listings.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<string> RunLottery(string requestId, string correlationId, string userId, long listingId, bool rerun = false)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        if ("|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|"))
        {
            var lotteryId = await _lotteryRepository.RunLottery(requestId, correlationId, userId, listingId, rerun);
            if (lotteryId > 0)
            {
                return lotteryId.ToString();
            }

            _logger.LogError($"Unable to run lottery - please check error message");
            return "LT003";
        }
        return "LT101";
    }

    public async Task<LotteryResultsViewModel> GetResults(string requestId, string correlationId, string userId, int lotteryId, int pageNo = 1, int pageSize = 1000)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);

        var lottery = await _lotteryRepository.GetOne(new Lottery { LotteryId = lotteryId });
        if (lottery == null) return null;

        var listing = await _listingsService.GetOne(requestId, correlationId, userId, lottery.ListingId);
        if (listing == null) return null;

        var pagedApplications = await _lotteryRepository.GetResultsPaged(lotteryId, pageNo, pageSize);
        return new LotteryResultsViewModel
        {
            LotteryId = lotteryId,
            LotteryDetails = lottery.ToViewModel(),
            ListingId = listing.ListingId,
            ListingDetails = listing.ToViewModel(),
            HousingApplications = pagedApplications.Records.Select(s => s.ToViewModel()),
            PageNo = pagedApplications.PagingInfo.PageNo,
            PageSize = pagedApplications.PagingInfo.PageSize,
            TotalPages = pagedApplications.PagingInfo.TotalPages,
            TotalRecords = pagedApplications.PagingInfo.TotalRecords,
            PageSizes = [ 100, 250, 500, 1000 ],
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<string> GetDownloadData(string requestId, string correlationId, string username, int lotteryId)
    {
        var dataTable = await _lotteryRepository.GetDownload(lotteryId);
        return dataTable.ToCsvFileContents();
    }
}