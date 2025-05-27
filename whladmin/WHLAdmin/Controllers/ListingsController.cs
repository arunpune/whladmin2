using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Extensions;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

public class ListingsController : Controller
{
    private readonly ILogger<ListingsController> _logger;
    private readonly IListingsService _listingsService;
    private readonly IMarketingAgentsService _agentsService;
    private readonly IMasterConfigService _masterConfigService;
    private readonly IMetadataService _metadataService;
    private readonly IUiHelperService _uiHelperService;

    public ListingsController(ILogger<ListingsController> logger, IListingsService listingsService
                                , IMarketingAgentsService agentsService, IMasterConfigService masterConfigService
                                , IMetadataService metadataService , IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listingsService = listingsService ?? throw new ArgumentNullException(nameof(listingsService));
        _agentsService = agentsService ?? throw new ArgumentNullException(nameof(agentsService));
        _masterConfigService = masterConfigService ?? throw new ArgumentNullException(nameof(masterConfigService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Index([FromHeader] string username
                                            , [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 1000)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        pageNo = pageNo < 1 ? 1 : pageNo;
        pageSize = pageSize < 100 ? 100 : pageSize;

        try
        {
            var model = await _listingsService.GetData(requestId, correlationId, username, pageNo, pageSize);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Details([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetOne(requestId, correlationId, username, listingId, true);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DetailsPreview([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetOne(requestId, correlationId, username, listingId, true);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> PrintableDetailsPreview([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetOne(requestId, correlationId, username, listingId, true);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> PrintableFormPreview([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetPrintableForm(listingId);
            if (model == null)
            {
                return NotFound();
            }

            model.Images = await _listingsService.GetImages(listingId);
            if ((model.Images?.Count() ?? 0) > 0)
            {
                var image = model.Images.FirstOrDefault(f => f.IsPrimary) ?? model.Images.First();
                ViewBag.ImageContents = (image.ThumbnailContents?.Length ?? 0) > 0 ? image.ThumbnailContents : image.Contents;
            }
            else
            {
                ViewBag.ImageContents = new PathString("/images/defaultlisting.png");
            }

            model.Documents = await _listingsService.GetDocuments(listingId);
            // if ((model.Documents?.Count() ?? 0) > 0)
            // {
            //     var image = model.Images.FirstOrDefault(f => f.IsPrimary) ?? model.Images.First();
            //     ViewBag.ImageContents = (image.ThumbnailContents?.Length ?? 0) > 0 ? image.ThumbnailContents : image.Contents;
            // }
            // else
            // {
            //     ViewBag.ImageContents = new PathString("/images/defaultlisting.png");
            // }

            model.Accessibilities = await _listingsService.GetListingAccessibilities(listingId);
            model.Declarations = await _listingsService.GetListingDeclarations(listingId);
            model.Disclosures = await _listingsService.GetListingDisclosures(listingId);

            model.PetDisclosure = model.Disclosures?.FirstOrDefault(f => f.Code.Equals("DISCPETYES", StringComparison.CurrentCultureIgnoreCase)
                                                                            || f.Code.Equals("DISCPETNO", StringComparison.CurrentCultureIgnoreCase))?.Text;
            model.Disclosures = model.Disclosures.Where(w => !w.Code.Equals("DISCPETYES", StringComparison.CurrentCultureIgnoreCase)
                                                                && !w.Code.Equals("DISCPETNO", StringComparison.CurrentCultureIgnoreCase));

            ViewBag.AddressText = _uiHelperService.ToAddressTextSingleLine(model);
            ViewBag.ApplicationEndDateText = _uiHelperService.ToDateTimeDisplayText(model.ApplicationEndDate, "MMMM d, yyyy", "t");
            ViewBag.LotteryDateText = _uiHelperService.ToDateTimeDisplayText(model.LotteryDate, "MMMM d, yyyy", "t");

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Add([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableListingViewModel()
            {
                ListingId = 0,
                ListingTypeCd = "RENTAL",
                ListingTypes = await _metadataService.GetListingTypes(false, true),
                ResaleInd = false,
                ListingAgeTypeCd = "ALL",
                ListingAgeTypes = await _metadataService.GetListingAgeTypes(),
                Name = "",
                Description = "",
                StreetAddress = "",
                StreetLine1 = "",
                StreetLine2 = "",
                StreetLine3 = "",
                City = "",
                StateCd = "",
                ZipCode = "",
                County = "",
                EsriX = "",
                EsriY = "",
                Municipality = "",
                MunicipalityUrl = "",
                SchoolDistrict = "",
                SchoolDistrictUrl = "",
                MapUrl = "",
                WebsiteUrl = "",
                PetsAllowedInd = false,
                PetsAllowedText = "",
                RentIncludesText = "Rent includes heat, hot water, and electric, including cooking. Income guidelines are subject to change.",
                CompletedOrInitialOccupancyYear = "",
                TermOfAffordability = "",
                StatusCd = "DRAFT",
                Statuses = await _metadataService.GetStatuses(true),
                Active = true,
                ArcGisSettings = await _masterConfigService.GetArcGisSettings()
            };
            return PartialView("_ListingAdd", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Add([FromHeader] string username, [FromForm] EditableListingViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = new ListingViewModel()
            {
                ListingId = 0,
                ListingTypeCd = model.ListingTypeCd,
                ResaleInd = model.ResaleInd,
                ListingAgeTypeCd = model.ListingAgeTypeCd,
                Name = model.Name,
                Description = model.Description,
                StreetLine1 = model.StreetLine1,
                StreetLine2 = model.StreetLine2,
                StreetLine3 = model.StreetLine3,
                City = model.City,
                StateCd = model.StateCd,
                ZipCode = model.ZipCode,
                County = model.County,
                EsriX = model.EsriX,
                EsriY = model.EsriY,
                Municipality = model.Municipality,
                MunicipalityUrl = model.MunicipalityUrl,
                SchoolDistrict = model.SchoolDistrict,
                SchoolDistrictUrl = model.SchoolDistrictUrl,
                MapUrl = model.MapUrl,
                WebsiteUrl = model.WebsiteUrl,
                WaitlistEligible = model.WaitlistEligible,
                MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt,
                MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt,
                MinHouseholdSize = model.MinHouseholdSize,
                MaxHouseholdSize = model.MaxHouseholdSize,
                PetsAllowedInd = model.PetsAllowedInd,
                PetsAllowedText = model.PetsAllowedText,
                RentIncludesText = model.RentIncludesText,
                CompletedOrInitialOccupancyYear = model.CompletedOrInitialOccupancyYear,
                TermOfAffordability = model.TermOfAffordability,
                StatusCd = model.StatusCd,
                Active = model.Active
            };
            if (DateTime.TryParse(model.ListingStartDate, out DateTime date))
            {
                listing.ListingStartDate = date;
            }
            if (DateTime.TryParse(model.ListingEndDate, out date))
            {
                listing.ListingEndDate = date;
            }
            if (DateTime.TryParse(model.ApplicationStartDate, out date))
            {
                listing.ApplicationStartDate = date;
            }
            if (DateTime.TryParse(model.ApplicationEndDate, out date))
            {
                listing.ApplicationEndDate = date;
            }
            if (DateTime.TryParse(model.LotteryDate, out date))
            {
                listing.LotteryDate = date;
            }
            if (DateTime.TryParse(model.WaitlistStartDate, out date))
            {
                listing.WaitlistStartDate = date;
            }
            if (DateTime.TryParse(model.WaitlistEndDate, out date))
            {
                listing.WaitlistEndDate = date;
            }
            var returnCode = await _listingsService.Add(correlationId, username, listing);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            model.ListingId = listing.ListingId;
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
            if (listing == null)
            {
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Listing not found" });
            }

            var model = new EditableListingViewModel()
            {
                ListingId = listingId,
                ListingTypeCd = listing.ListingTypeCd,
                ListingTypes = await _metadataService.GetListingTypes(false, true),
                ResaleInd = listing.ResaleInd,
                ListingAgeTypeCd = listing.ListingAgeTypeCd,
                ListingAgeTypes = await _metadataService.GetListingAgeTypes(),
                Name = listing.Name,
                Description = listing.Description,
                StreetAddress = listing.ToAddressTextSingleLine(),
                StreetLine1 = listing.StreetLine1,
                StreetLine2 = listing.StreetLine2,
                StreetLine3 = listing.StreetLine3,
                City = listing.City,
                StateCd = listing.StateCd,
                ZipCode = listing.ZipCode,
                County = listing.County,
                EsriX = listing.EsriX,
                EsriY = listing.EsriY,
                Municipality = listing.Municipality,
                MunicipalityUrl = listing.MunicipalityUrl,
                SchoolDistrict = listing.SchoolDistrict,
                SchoolDistrictUrl = listing.SchoolDistrictUrl,
                MapUrl = listing.MapUrl,
                WebsiteUrl = listing.WebsiteUrl,
                ListingStartDate = listing.ListingStartDate.HasValue ? listing.ListingStartDate.Value.ToString("yyyy-MM-dd") : null,
                ListingEndDate = listing.ListingEndDate.HasValue ? listing.ListingEndDate.Value.ToString("yyyy-MM-dd") : null,
                ApplicationStartDate = listing.ApplicationStartDate.HasValue ? listing.ApplicationStartDate.Value.ToString("yyyy-MM-dd") : null,
                ApplicationEndDate = listing.ApplicationEndDate.HasValue ? listing.ApplicationEndDate.Value.ToString("yyyy-MM-dd") : null,
                LotteryDate = listing.LotteryDate.HasValue ? listing.LotteryDate.Value.ToString("yyyy-MM-dd") : null,
                WaitlistEligible = listing.WaitlistEligible,
                WaitlistStartDate = listing.WaitlistStartDate.HasValue ? listing.WaitlistStartDate.Value.ToString("yyyy-MM-dd") : null,
                WaitlistEndDate = listing.WaitlistEndDate.HasValue ? listing.WaitlistEndDate.Value.ToString("yyyy-MM-dd") : null,
                MinHouseholdIncomeAmt = listing.MinHouseholdIncomeAmt,
                MaxHouseholdIncomeAmt = listing.MaxHouseholdIncomeAmt,
                MinHouseholdSize = listing.MinHouseholdSize,
                MaxHouseholdSize = listing.MaxHouseholdSize,
                PetsAllowedInd = listing.PetsAllowedInd,
                PetsAllowedText = listing.PetsAllowedText,
                RentIncludesText = listing.RentIncludesText,
                CompletedOrInitialOccupancyYear = listing.CompletedOrInitialOccupancyYear,
                TermOfAffordability = listing.TermOfAffordability,
                StatusCd = listing.StatusCd,
                Statuses = await _metadataService.GetStatuses(true),
                Active = listing.Active,
                ArcGisSettings = await _masterConfigService.GetArcGisSettings()
            };
            return PartialView("_ListingEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromForm] EditableListingViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = new ListingViewModel()
            {
                ListingId = model.ListingId,
                ListingTypeCd = model.ListingTypeCd,
                ResaleInd = model.ResaleInd,
                ListingAgeTypeCd = model.ListingAgeTypeCd,
                Name = model.Name,
                Description = model.Description,
                StreetLine1 = model.StreetLine1,
                StreetLine2 = model.StreetLine2,
                StreetLine3 = model.StreetLine3,
                City = model.City,
                StateCd = model.StateCd,
                ZipCode = model.ZipCode,
                County = model.County,
                EsriX = model.EsriX,
                EsriY = model.EsriY,
                Municipality = model.Municipality,
                MunicipalityUrl = model.MunicipalityUrl,
                SchoolDistrict = model.SchoolDistrict,
                SchoolDistrictUrl = model.SchoolDistrictUrl,
                MapUrl = model.MapUrl,
                WebsiteUrl = model.WebsiteUrl,
                WaitlistEligible = model.WaitlistEligible,
                MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt,
                MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt,
                MinHouseholdSize = model.MinHouseholdSize,
                MaxHouseholdSize = model.MaxHouseholdSize,
                PetsAllowedInd = model.PetsAllowedInd,
                PetsAllowedText = model.PetsAllowedText,
                RentIncludesText = model.RentIncludesText,
                CompletedOrInitialOccupancyYear = model.CompletedOrInitialOccupancyYear,
                TermOfAffordability = model.TermOfAffordability,
                StatusCd = model.StatusCd,
                Active = model.Active
            };
            if (DateTime.TryParse(model.ListingStartDate, out DateTime date))
            {
                listing.ListingStartDate = date;
            }
            if (DateTime.TryParse(model.ListingEndDate, out date))
            {
                listing.ListingEndDate = date;
            }
            if (DateTime.TryParse(model.ApplicationStartDate, out date))
            {
                listing.ApplicationStartDate = date;
            }
            if (DateTime.TryParse(model.ApplicationEndDate, out date))
            {
                listing.ApplicationEndDate = date;
            }
            if (DateTime.TryParse(model.LotteryDate, out date))
            {
                listing.LotteryDate = date;
            }
            if (DateTime.TryParse(model.WaitlistStartDate, out date))
            {
                listing.WaitlistStartDate = date;
            }
            if (DateTime.TryParse(model.WaitlistEndDate, out date))
            {
                listing.WaitlistEndDate = date;
            }
            var returnCode = await _listingsService.Update(correlationId, username, listing);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Delete([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.Delete(correlationId, username, listingId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAddress([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
            if (listing == null)
            {
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Listing not found" });
            }

            var model = new EditableListingAddressViewModel()
            {
                ListingId = listingId,
                StreetAddress = listing.ToAddressTextSingleLine(),
                StreetLine1 = listing.StreetLine1,
                StreetLine2 = listing.StreetLine2,
                StreetLine3 = listing.StreetLine3,
                City = listing.City,
                StateCd = listing.StateCd,
                ZipCode = listing.ZipCode,
                County = listing.County,
                EsriX = listing.EsriX,
                EsriY = listing.EsriY,
                Municipality = listing.Municipality,
                MunicipalityUrl = listing.MunicipalityUrl,
                SchoolDistrict = listing.SchoolDistrict,
                SchoolDistrictUrl = listing.SchoolDistrictUrl,
                MapUrl = listing.MapUrl,
            };
            return PartialView("_AddressEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAddress([FromHeader] string username, [FromForm] EditableListingAddressViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var addressViewModel = new ListingViewModel()
            {
                ListingId = model.ListingId,
                StreetLine1 = model.StreetLine1,
                StreetLine2 = model.StreetLine2,
                StreetLine3 = model.StreetLine3,
                City = model.City,
                StateCd = model.StateCd,
                ZipCode = model.ZipCode,
                County = model.County,
                EsriX = model.EsriX,
                EsriY = model.EsriY,
                Municipality = model.Municipality,
                MunicipalityUrl = model.MunicipalityUrl,
                SchoolDistrict = model.SchoolDistrict,
                SchoolDistrictUrl = model.SchoolDistrictUrl,
                MapUrl = model.MapUrl,
            };

            var returnCode = await _listingsService.UpdateAddress(correlationId, username, addressViewModel);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDates([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
            if (listing == null)
            {
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Listing not found" });
            }

            var model = new EditableListingDatesViewModel()
            {
                ListingId = listingId,
                ListingStartDate = _uiHelperService.ToDateEditorFormat(listing.ListingStartDate),
                ListingEndDate = _uiHelperService.ToDateEditorFormat(listing.ListingEndDate),
                ApplicationStartDate = _uiHelperService.ToDateEditorFormat(listing.ApplicationStartDate),
                ApplicationStartTime = listing.ApplicationStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? listing.ApplicationStartDate.Value.ToString("HH:mm") : null,
                ApplicationEndDate = _uiHelperService.ToDateEditorFormat(listing.ApplicationEndDate),
                ApplicationEndTime = _uiHelperService.ToTimeEditorFormat(listing.ApplicationEndDate, "HH:mm"),
                LotteryEligible = listing.LotteryEligible,
                LotteryDate = _uiHelperService.ToDateEditorFormat(listing.LotteryDate),
                LotteryTime = _uiHelperService.ToTimeEditorFormat(listing.LotteryDate, "HH:mm"),
                WaitlistEligible = listing.WaitlistEligible,
                WaitlistStartDate = _uiHelperService.ToDateEditorFormat(listing.WaitlistStartDate),
                WaitlistStartTime = _uiHelperService.ToTimeEditorFormat(listing.WaitlistStartDate, "HH:mm"),
                WaitlistEndDate = _uiHelperService.ToDateEditorFormat(listing.WaitlistEndDate),
                WaitlistEndTime = _uiHelperService.ToTimeEditorFormat(listing.WaitlistEndDate, "HH:mm"),
            };
            return PartialView("_DatesEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDates([FromHeader] string username, [FromForm] EditableListingDatesViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            DateTime.TryParse(model.ListingStartDate, out var listingStartDate);
            DateTime.TryParse(model.ListingEndDate, out var listingEndDate);
            DateTime.TryParse(model.ApplicationStartDate, out var applicationStartDate);
            DateTime.TryParse(model.ApplicationStartTime, out var applicationStartTime);
            DateTime.TryParse(model.ApplicationEndDate, out var applicationEndDate);
            DateTime.TryParse(model.ApplicationEndTime, out var applicationEndTime);
            DateTime.TryParse(model.LotteryDate, out var lotteryDate);
            DateTime.TryParse(model.LotteryTime, out var lotteryTime);
            DateTime.TryParse(model.WaitlistStartDate, out var waitlistStartDate);
            DateTime.TryParse(model.WaitlistStartTime, out var waitlistStartTime);
            DateTime.TryParse(model.WaitlistEndDate, out var waitlistEndDate);
            DateTime.TryParse(model.WaitlistEndTime, out var waitlistEndTime);

            var datesViewModel = new ListingViewModel()
            {
                ListingId = model.ListingId,
                ListingStartDate = listingStartDate.Date > DateTime.MinValue.Date ? listingStartDate : null,
                ListingEndDate = listingEndDate >= listingStartDate.Date ? listingEndDate : null,
                ApplicationStartDate = applicationStartDate.Date >= listingStartDate.Date ? new DateTime(applicationStartDate.Year, applicationStartDate.Month, applicationStartDate.Day, applicationStartTime.Hour, applicationStartTime.Minute, 0) : null,
                ApplicationEndDate = applicationEndDate.Date > applicationStartDate ? new DateTime(applicationEndDate.Year, applicationEndDate.Month, applicationEndDate.Day, applicationEndTime.Hour, applicationEndTime.Minute, 0) : null,
                LotteryEligible = model.LotteryEligible,
                LotteryDate = lotteryDate.Ticks >= new DateTime(applicationEndDate.Year, applicationEndDate.Month, applicationEndDate.Day, applicationEndTime.Hour, applicationEndTime.Minute, 0).Ticks ? new DateTime(lotteryDate.Year, lotteryDate.Month, lotteryDate.Day, lotteryTime.Hour, lotteryTime.Minute, 0) : null,
                WaitlistEligible = model.WaitlistEligible,
                WaitlistStartDate = waitlistStartDate.Date >= applicationEndDate.Date && waitlistStartTime.Ticks >= applicationEndTime.Ticks ? new DateTime(waitlistStartDate.Year, waitlistStartDate.Month, waitlistStartDate.Day, waitlistStartTime.Hour, waitlistStartTime.Minute, 0) : new DateTime(applicationEndDate.Year, applicationEndDate.Month, applicationEndDate.Day, applicationEndTime.Hour, applicationEndTime.Minute, 0),
                WaitlistEndDate = waitlistEndDate.Date >= waitlistStartDate.Date && waitlistStartTime.Ticks >= applicationEndTime.Ticks ? new DateTime(waitlistEndDate.Year, waitlistEndDate.Month, waitlistEndDate.Day, waitlistEndTime.Hour, waitlistEndTime.Minute, 0) : null,
            };

            var returnCode = await _listingsService.UpdateDates(correlationId, username, datesViewModel);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditPetsAllowed([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
            if (listing == null)
            {
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Listing not found" });
            }

            var model = new EditableListingPetsAllowedViewModel()
            {
                ListingId = listingId,
                PetsAllowedInd = listing.PetsAllowedInd,
                PetsAllowedText = listing.PetsAllowedText
            };
            return PartialView("_PetsAllowedEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditPetsAllowed([FromHeader] string username, [FromForm] EditableListingPetsAllowedViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            model.PetsAllowedText = (model.PetsAllowedText ?? string.Empty).Trim();
            var petsViewModel = new ListingViewModel()
            {
                ListingId = model.ListingId,
                PetsAllowedInd = model.PetsAllowedInd,
                PetsAllowedText = string.IsNullOrEmpty(model.PetsAllowedText) ? null : model.PetsAllowedText,
            };

            var returnCode = await _listingsService.UpdatePetsAllowed(correlationId, username, petsViewModel);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditMarketingAgent([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
            if (listing == null)
            {
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Listing not found" });
            }

            var agents = new List<MarketingAgentViewModel>
            {
                new MarketingAgentViewModel { AgentId = 0, Name = "Select One" }
            };
            agents.AddRange(await _agentsService.GetAll(requestId, correlationId));

            var model = new EditableListingMarketingAgentViewModel()
            {
                ListingId = listingId,
                MarketingAgentInd = listing.MarketingAgentInd,
                MarketingAgentId = listing.MarketingAgentId,
                MarketingAgentApplicationLink = listing.MarketingAgentApplicationLink,
                Agents = agents
            };
            return PartialView("_MarketingAgentEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditMarketingAgent([FromHeader] string username, [FromForm] EditableListingMarketingAgentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            model.MarketingAgentApplicationLink = (model.MarketingAgentApplicationLink ?? string.Empty).Trim();
            var agentViewModel = new ListingViewModel()
            {
                ListingId = model.ListingId,
                MarketingAgentInd = model.MarketingAgentInd,
                MarketingAgentId = model.MarketingAgentId,
                MarketingAgentApplicationLink = string.IsNullOrEmpty(model.MarketingAgentApplicationLink) ? null : model.MarketingAgentApplicationLink,
            };

            var returnCode = await _listingsService.UpdateMarketingAgent(requestId, correlationId, username, agentViewModel);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDocumentsReqd([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var documentTypes = await _listingsService.GetDocumentTypesForEdit(listingId);
            var selectedDocumentTypeIds = documentTypes.Where(w => w.Selected).Select(s => s.DocumentTypeId);
            var documentTypeIds = (selectedDocumentTypeIds?.Count() ?? 0) > 0 ? string.Join(",", selectedDocumentTypeIds) : string.Empty;
            var model = new EditableListingDocumentsReqdViewModel()
            {
                ListingId = listingId,
                DocumentTypes = documentTypes,
                DocumentTypeIds = documentTypeIds
            };
            return PartialView("_DocumentsReqdEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDocumentsReqd([FromHeader] string username, [FromForm] EditableListingDocumentsReqdViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.SaveDocumentTypes(correlationId, username, model.ListingId, model.DocumentTypeIds);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult AddImage([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableListingImageViewModel()
            {
                Active = true,
                Contents = "",
                ImageId = 0,
                IsPrimary = false,
                DisplayOnListingsPageInd = true,
                ListingId = listingId,
                MimeType = "",
                ThumbnailContents = "",
                Title = "",
            };
            return PartialView("_ImageEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddImage([FromHeader] string username, [FromForm] EditableListingImageViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var image = new ListingImageViewModel()
            {
                ImageId = 0,
                ListingId = model.ListingId,
                Title = model.Title,
                Contents = model.Contents,
                ThumbnailContents = model.ThumbnailContents,
                MimeType = model.MimeType,
                IsPrimary = model.IsPrimary,
                DisplayOnListingsPageInd = model.DisplayOnListingsPageInd,
                Active = true
            };
            var returnCode = await _listingsService.AddImage(correlationId, username, image);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditImage([FromHeader] string username, [FromQuery] int imageId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var image = await _listingsService.GetImage(imageId);

            var model = new EditableListingImageViewModel()
            {
                Active = image.Active,
                Contents = image.Contents,
                ImageId = image.ImageId,
                IsPrimary = image.IsPrimary,
                DisplayOnListingsPageInd = image.DisplayOnListingsPageInd,
                ListingId = image.ListingId,
                MimeType = image.MimeType,
                ThumbnailContents = image.ThumbnailContents,
                Title = image.Title,
            };
            return PartialView("_ImageEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditImage([FromHeader] string username, [FromForm] EditableListingImageViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var image = new ListingImageViewModel()
            {
                ImageId = model.ImageId,
                ListingId = model.ListingId,
                Title = model.Title,
                IsPrimary = model.IsPrimary,
                DisplayOnListingsPageInd = model.DisplayOnListingsPageInd,
                Active = true
            };
            var returnCode = await _listingsService.UpdateImage(correlationId, username, image);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> DeleteImage([FromHeader] string username, [FromQuery] int imageId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.DeleteImage(correlationId, username, imageId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult AddDocument([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableListingDocumentViewModel()
            {
                Active = true,
                Contents = "",
                DocumentId = 0,
                DisplayOnListingsPageInd = true,
                FileName = "",
                ListingId = listingId,
                MimeType = "",
                Title = "",
            };
            return PartialView("_DocumentEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddDocument([FromHeader] string username, [FromForm] EditableListingDocumentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var document = new ListingDocumentViewModel()
            {
                DocumentId = 0,
                ListingId = model.ListingId,
                Title = model.Title,
                FileName = model.FileName,
                Contents = model.Contents,
                MimeType = model.MimeType,
                DisplayOnListingsPageInd = model.DisplayOnListingsPageInd,
                Active = true
            };
            var returnCode = await _listingsService.AddDocument(correlationId, username, document);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDocument([FromHeader] string username, [FromQuery] int documentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var document = await _listingsService.GetDocument(documentId);

            var model = new EditableListingDocumentViewModel()
            {
                Active = document.Active,
                Contents = document.Contents,
                DocumentId = document.DocumentId,
                DisplayOnListingsPageInd = document.DisplayOnListingsPageInd,
                ListingId = document.ListingId,
                MimeType = document.MimeType,
                Title = document.Title,
            };
            return PartialView("_DocumentEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDocument([FromHeader] string username, [FromForm] EditableListingDocumentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var document = new ListingDocumentViewModel()
            {
                DocumentId = model.DocumentId,
                ListingId = model.ListingId,
                Title = model.Title,
                DisplayOnListingsPageInd = model.DisplayOnListingsPageInd,
                Active = true
            };
            var returnCode = await _listingsService.UpdateDocument(correlationId, username, document);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> DeleteDocument([FromHeader] string username, [FromQuery] int documentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.DeleteDocument(correlationId, username, documentId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> ViewDocument([FromHeader] string username, [FromQuery] int documentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetDocument(documentId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Code = "L251",
                    Message = _listingsService.GetMessage("L251"),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            if (model.Contents.Contains(";base64,"))
            {
                model.Contents = model.Contents.Substring(model.Contents.IndexOf(";base64,") + 8);
            }
            return File(Convert.FromBase64String(model.Contents), model.MimeType, model.FileName);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddUnit([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var existingListing = await _listingsService.GetOne(requestId, correlationId, username, listingId);

            var model = new EditableListingUnitViewModel()
            {
                Active = true,
                AreaMedianIncomePct = 0,
                AssetLimitAmt = 0L,
                BathroomCnt = 0,
                BathroomCntPart = 0,
                BedroomCnt = 0,
                EstimatedPriceAmt = 0L,
                IsRental = existingListing.IsRental,
                ListingId = listingId,
                MaxHouseholdIncomeAmt1 = 0L,
                MaxHouseholdIncomeAmt2 = 0L,
                MaxHouseholdIncomeAmt3 = 0L,
                MaxHouseholdIncomeAmt4 = 0L,
                MaxHouseholdIncomeAmt5 = 0L,
                MaxHouseholdIncomeAmt6 = 0L,
                MaxHouseholdIncomeAmt7 = 0L,
                MinHouseholdIncomeAmt1 = 0L,
                MinHouseholdIncomeAmt2 = 0L,
                MinHouseholdIncomeAmt3 = 0L,
                MinHouseholdIncomeAmt4 = 0L,
                MinHouseholdIncomeAmt5 = 0L,
                MinHouseholdIncomeAmt6 = 0L,
                MinHouseholdIncomeAmt7 = 0L,
                MinHouseholdIncomeAmt8 = 0L,
                MonthlyInsuranceAmt = 0L,
                MonthlyMaintenanceAmt = 0L,
                MonthlyRentAmt = 0L,
                MonthlyTaxesAmt = 0L,
                SquareFootage = 0,
                SubsidyAmt = 0L,
                UnitId = 0,
                UnitsAvailableCnt = 0,
                UnitTypeCd = "",
                UnitTypes = await _metadataService.GetUnitTypes(true),
                BathroomPartOptions = await _metadataService.GetBathroomPartOptions()
            };
            return PartialView("_UnitEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddUnit([FromHeader] string username, [FromForm] EditableListingUnitViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var unit = new ListingUnitViewModel()
            {
                UnitId = 0,
                ListingId = model.ListingId,
                AreaMedianIncomePct = model.AreaMedianIncomePct,
                AssetLimitAmt = model.AssetLimitAmt,
                BathroomCnt = model.BathroomCnt,
                BathroomCntPart = model.BathroomCntPart,
                BedroomCnt = model.BedroomCnt,
                EstimatedPriceAmt = model.EstimatedPriceAmt,
                MonthlyInsuranceAmt = model.MonthlyInsuranceAmt,
                MonthlyMaintenanceAmt = model.MonthlyMaintenanceAmt,
                MonthlyRentAmt = model.MonthlyRentAmt,
                MonthlyTaxesAmt = model.MonthlyTaxesAmt,
                SquareFootage = model.SquareFootage,
                SubsidyAmt = model.SubsidyAmt,
                UnitHouseholds = [],
                UnitsAvailableCnt = model.UnitsAvailableCnt,
                UnitTypeCd = model.UnitTypeCd,
                Active = true
            };

            var unitHouseholds = new List<ListingUnitHouseholdViewModel>();
            if (model.MinHouseholdIncomeAmt1 > 0L || model.MaxHouseholdIncomeAmt1 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 1,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt1,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt1
                });
            }
            if (model.MinHouseholdIncomeAmt2 > 0L || model.MaxHouseholdIncomeAmt2 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 2,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt2,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt2
                });
            }
            if (model.MinHouseholdIncomeAmt3 > 0L || model.MaxHouseholdIncomeAmt3 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 3,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt3,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt3
                });
            }
            if (model.MinHouseholdIncomeAmt4 > 0L || model.MaxHouseholdIncomeAmt4 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 4,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt4,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt4
                });
            }
            if (model.MinHouseholdIncomeAmt5 > 0L || model.MaxHouseholdIncomeAmt5 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 5,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt5,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt5
                });
            }
            if (model.MinHouseholdIncomeAmt6 > 0L || model.MaxHouseholdIncomeAmt6 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 6,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt6,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt6
                });
            }
            if (model.MinHouseholdIncomeAmt7 > 0L || model.MaxHouseholdIncomeAmt7 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 7,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt7,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt7
                });
            }
            if (model.MinHouseholdIncomeAmt8 > 0L || model.MaxHouseholdIncomeAmt8 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 8,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt8,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt8
                });
            }
            unit.UnitHouseholds = unitHouseholds;

            var returnCode = await _listingsService.AddUnit(correlationId, username, unit);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditUnit([FromHeader] string username, [FromQuery] int unitId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var unit = await _listingsService.GetUnit(unitId);
            var households = await _listingsService.GetHouseholds(unit.ListingId);
            var listing = await _listingsService.GetOne(requestId, correlationId, username, unit.ListingId);

            var model = new EditableListingUnitViewModel()
            {
                Active = unit.Active,
                AreaMedianIncomePct = unit.AreaMedianIncomePct,
                AssetLimitAmt = unit.AssetLimitAmt,
                BathroomCnt = unit.BathroomCnt,
                BathroomCntPart = unit.BathroomCntPart,
                BedroomCnt = unit.BedroomCnt,
                EstimatedPriceAmt = unit.EstimatedPriceAmt,
                IsRental = listing.IsRental,
                ListingId = unit.ListingId,
                MaxHouseholdIncomeAmt1 = 0L,
                MaxHouseholdIncomeAmt2 = 0L,
                MaxHouseholdIncomeAmt3 = 0L,
                MaxHouseholdIncomeAmt4 = 0L,
                MaxHouseholdIncomeAmt5 = 0L,
                MaxHouseholdIncomeAmt6 = 0L,
                MaxHouseholdIncomeAmt7 = 0L,
                MinHouseholdIncomeAmt1 = 0L,
                MinHouseholdIncomeAmt2 = 0L,
                MinHouseholdIncomeAmt3 = 0L,
                MinHouseholdIncomeAmt4 = 0L,
                MinHouseholdIncomeAmt5 = 0L,
                MinHouseholdIncomeAmt6 = 0L,
                MinHouseholdIncomeAmt7 = 0L,
                MinHouseholdIncomeAmt8 = 0L,
                MonthlyInsuranceAmt = unit.MonthlyInsuranceAmt,
                MonthlyMaintenanceAmt = unit.MonthlyMaintenanceAmt,
                MonthlyRentAmt = unit.MonthlyRentAmt,
                MonthlyTaxesAmt = unit.MonthlyTaxesAmt,
                SquareFootage = unit.SquareFootage,
                SubsidyAmt = unit.SubsidyAmt,
                UnitId = unitId,
                UnitsAvailableCnt = unit.UnitsAvailableCnt,
                UnitTypeCd = unit.UnitTypeCd,
                UnitTypes = await _metadataService.GetUnitTypes(true),
                BathroomPartOptions = await _metadataService.GetBathroomPartOptions()
            };

            foreach (var household in households.Where(w => w.UnitId == unitId))
            {
                switch (household.HouseholdSize)
                {
                    case 1:
                        model.MinHouseholdIncomeAmt1 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt1 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 2:
                        model.MinHouseholdIncomeAmt2 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt2 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 3:
                        model.MinHouseholdIncomeAmt3 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt3 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 4:
                        model.MinHouseholdIncomeAmt4 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt4 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 5:
                        model.MinHouseholdIncomeAmt5 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt5 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 6:
                        model.MinHouseholdIncomeAmt6 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt6 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 7:
                        model.MinHouseholdIncomeAmt7 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt7 = household.MaxHouseholdIncomeAmt;
                        break;
                    case 8:
                        model.MinHouseholdIncomeAmt8 = household.MinHouseholdIncomeAmt;
                        model.MaxHouseholdIncomeAmt8 = household.MaxHouseholdIncomeAmt;
                        break;
                }
            }

            return PartialView("_UnitEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditUnit([FromHeader] string username, [FromForm] EditableListingUnitViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var unit = new ListingUnitViewModel()
            {
                UnitId = model.UnitId,
                ListingId = model.ListingId,
                AreaMedianIncomePct = model.AreaMedianIncomePct,
                AssetLimitAmt = model.AssetLimitAmt,
                BathroomCnt = model.BathroomCnt,
                BathroomCntPart = model.BathroomCntPart,
                BedroomCnt = model.BedroomCnt,
                EstimatedPriceAmt = model.EstimatedPriceAmt,
                MonthlyInsuranceAmt = model.MonthlyInsuranceAmt,
                MonthlyMaintenanceAmt = model.MonthlyMaintenanceAmt,
                MonthlyRentAmt = model.MonthlyRentAmt,
                MonthlyTaxesAmt = model.MonthlyTaxesAmt,
                SquareFootage = model.SquareFootage,
                SubsidyAmt = model.SubsidyAmt,
                UnitHouseholds = [],
                UnitsAvailableCnt = model.UnitsAvailableCnt,
                UnitTypeCd = model.UnitTypeCd,
                Active = true
            };

            var unitHouseholds = new List<ListingUnitHouseholdViewModel>();
            if (model.MinHouseholdIncomeAmt1 > 0L || model.MaxHouseholdIncomeAmt1 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 1,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt1,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt1
                });
            }
            if (model.MinHouseholdIncomeAmt2 > 0L || model.MaxHouseholdIncomeAmt2 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 2,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt2,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt2
                });
            }
            if (model.MinHouseholdIncomeAmt3 > 0L || model.MaxHouseholdIncomeAmt3 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 3,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt3,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt3
                });
            }
            if (model.MinHouseholdIncomeAmt4 > 0L || model.MaxHouseholdIncomeAmt4 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 4,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt4,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt4
                });
            }
            if (model.MinHouseholdIncomeAmt5 > 0L || model.MaxHouseholdIncomeAmt5 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 5,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt5,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt5
                });
            }
            if (model.MinHouseholdIncomeAmt6 > 0L || model.MaxHouseholdIncomeAmt6 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 6,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt6,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt6
                });
            }
            if (model.MinHouseholdIncomeAmt7 > 0L || model.MaxHouseholdIncomeAmt7 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 7,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt7,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt7
                });
            }
            if (model.MinHouseholdIncomeAmt8 > 0L || model.MaxHouseholdIncomeAmt8 > 0L)
            {
                unitHouseholds.Add(new ListingUnitHouseholdViewModel()
                {
                    HouseholdId = 0,
                    HouseholdSize = 8,
                    MinHouseholdIncomeAmt = model.MinHouseholdIncomeAmt8,
                    MaxHouseholdIncomeAmt = model.MaxHouseholdIncomeAmt8
                });
            }
            unit.UnitHouseholds = unitHouseholds;

            var returnCode = await _listingsService.UpdateUnit(correlationId, username, unit);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> DeleteUnit([FromHeader] string username, [FromQuery] int unitId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.DeleteUnit(correlationId, username, unitId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAmenities([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var amenities = await _listingsService.GetAmenitiesForEdit(listingId);
            var selectedAmenityIds = amenities.Where(w => w.Selected).Select(s => s.AmenityId);
            var amenityIds = (selectedAmenityIds?.Count() ?? 0) > 0 ? string.Join(",", selectedAmenityIds) : string.Empty;
            var model = new EditableListingAmenitiesViewModel()
            {
                ListingId = listingId,
                Amenities = amenities,
                AmenityIds = amenityIds
            };
            return PartialView("_AmenityEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAmenities([FromHeader] string username, [FromForm] EditableListingAmenitiesViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.SaveAmenities(correlationId, username, model.ListingId, model.AmenityIds);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditFundingSources([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var fundingSources = await _listingsService.GetFundingSourcesForEdit(listingId);
            var selectedFundingSourceIds = fundingSources.Where(w => w.Selected).Select(s => s.FundingSourceId);
            var fundingSourceIds = (selectedFundingSourceIds?.Count() ?? 0) > 0 ? string.Join(",", selectedFundingSourceIds) : string.Empty;
            var model = new EditableListingFundingSourcesViewModel()
            {
                ListingId = listingId,
                FundingSources = fundingSources,
                FundingSourceIds = fundingSourceIds
            };
            return PartialView("_FundingSourceEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditFundingSources([FromHeader] string username, [FromForm] EditableListingFundingSourcesViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.SaveFundingSources(correlationId, username, model.ListingId, model.FundingSourceIds);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAccessibilities([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var accessibilities = await _metadataService.GetAccessibilities();
            var selectedAccessibilities = await _listingsService.GetListingAccessibilities(listingId);
            var selectedAccessibilityCds = selectedAccessibilities.Select(s => s.Key);
            var accessibilityCds = (selectedAccessibilityCds?.Count() ?? 0) > 0 ? string.Join(",", selectedAccessibilityCds) : string.Empty;
            var model = new EditableListingAccessibilitiesViewModel()
            {
                ListingId = listingId,
                Accessibilities = accessibilities,
                AccessibilityCds = accessibilityCds
            };
            return PartialView("_AccessibilityEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditAccessibilities([FromHeader] string username, [FromForm] EditableListingAccessibilitiesViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.SaveAccessibilities(correlationId, username, model.ListingId, model.AccessibilityCds);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult AddDeclaration([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableListingDeclarationViewModel()
            {
                DeclarationId = 0,
                ListingId = listingId,
                SortOrder = 0,
                Text = "",
            };
            return PartialView("_DeclarationEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddDeclaration([FromHeader] string username, [FromForm] EditableListingDeclarationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var declaration = new ListingDeclarationViewModel()
            {
                DeclarationId = 0,
                ListingId = model.ListingId,
                Text = model.Text,
                SortOrder = model.SortOrder,
                Active = true
            };
            var returnCode = await _listingsService.AddDeclaration(correlationId, username, declaration);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDeclaration([FromHeader] string username, [FromQuery] int declarationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var declaration = await _listingsService.GetListingDeclaration(declarationId);
            if (declaration == null)
            {
                return NotFound();
            }
            var model = new EditableListingDeclarationViewModel()
            {
                DeclarationId = declarationId,
                ListingId = declaration.ListingId,
                SortOrder = declaration.SortOrder,
                Text = declaration.Text
            };
            return PartialView("_DeclarationEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDeclaration([FromHeader] string username, [FromForm] EditableListingDeclarationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var declaration = new ListingDeclarationViewModel()
            {
                DeclarationId = model.DeclarationId,
                ListingId = model.ListingId,
                Text = model.Text,
                SortOrder = model.SortOrder,
                Active = true
            };
            var returnCode = await _listingsService.UpdateDeclaration(correlationId, username, declaration);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> DeleteDeclaration([FromHeader] string username, [FromQuery] int declarationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.DeleteDeclaration(correlationId, username, declarationId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult AddDisclosure([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableListingDisclosureViewModel()
            {
                DisclosureId = 0,
                ListingId = listingId,
                SortOrder = 0,
                Text = "",
            };
            return PartialView("_DisclosureEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("View", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> AddDisclosure([FromHeader] string username, [FromForm] EditableListingDisclosureViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var disclosure = new ListingDisclosureViewModel()
            {
                DisclosureId = 0,
                ListingId = model.ListingId,
                Text = model.Text,
                SortOrder = model.SortOrder,
                Active = true
            };
            var returnCode = await _listingsService.AddDisclosure(correlationId, username, disclosure);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDisclosure([FromHeader] string username, [FromQuery] int disclosureId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var disclosure = await _listingsService.GetListingDisclosure(disclosureId);
            if (disclosure == null)
            {
                return NotFound();
            }
            var model = new EditableListingDisclosureViewModel()
            {
                DisclosureId = disclosureId,
                ListingId = disclosure.ListingId,
                SortOrder = disclosure.SortOrder,
                Text = disclosure.Text
            };
            return PartialView("_DisclosureEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> EditDisclosure([FromHeader] string username, [FromForm] EditableListingDisclosureViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var disclosure = new ListingDisclosureViewModel()
            {
                DisclosureId = model.DisclosureId,
                ListingId = model.ListingId,
                Text = model.Text,
                SortOrder = model.SortOrder,
                Active = true
            };
            var returnCode = await _listingsService.UpdateDisclosure(correlationId, username, disclosure);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> DeleteDisclosure([FromHeader] string username, [FromQuery] int disclosureId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _listingsService.DeleteDisclosure(correlationId, username, disclosureId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Submit([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _listingsService.Submit(requestId, correlationId, siteUrl, username, listingId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Revise([FromHeader] string username, [FromForm] EditableListingActionViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _listingsService.Revise(requestId, correlationId, siteUrl, username, model);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Publish([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _listingsService.Publish(requestId, correlationId, siteUrl, username, listingId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Unpublish([FromHeader] string username, [FromForm] EditableListingActionViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _listingsService.Unpublish(requestId, correlationId, siteUrl, username, model);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _listingsService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> AffordabilityAnalysis([FromHeader] string username, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetForAffordabilityAnalysis(requestId, correlationId, listingId);
            return PartialView("_AffordabilityAnalysisViewer", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> AffordabilityDetails([FromHeader] string username, [FromQuery] int unitId, [FromQuery] decimal rate, [FromQuery] int term)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetAffordabilityAnalysis(requestId, correlationId, unitId, rate, term);
            return Ok(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }
}