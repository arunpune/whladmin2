using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Services;

public class ListingServiceTests
{
    private readonly Mock<ILogger<ListingService>> _logger = new();
    private readonly Mock<IListingRepository> _listingRepository = new();
    private readonly Mock<IListingImageRepository> _imageRepository = new();
    private readonly Mock<IListingUnitRepository> _unitRepository = new();
    private readonly Mock<IListingUnitHouseholdRepository> _unitHouseholdRepository = new();
    private readonly Mock<IListingAmenityRepository> _amenityRepository = new();
    private readonly Mock<IListingAccessibilityRepository> _accessibilityRepository = new();
    private readonly Mock<IListingDeclarationRepository> _declarationRepository = new();
    private readonly Mock<IListingDisclosureRepository> _disclosureRepository = new();
    private readonly Mock<IListingDocumentRepository> _documentRepository = new();
    private readonly Mock<IListingDocumentTypeRepository> _documentTypeRepository = new();
    private readonly Mock<IListingFundingSourceRepository> _fundingSourceRepository = new();
    private readonly Mock<IHousingApplicationRepository> _applicationRepository = new();
    private readonly Mock<IUiHelperService> _uiHelperService = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IAmortizationsService> _amortizationsService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ListingService(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object, null, null, null, null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, null, null, null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, null, null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        null, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, null, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, _documentTypeRepository.Object, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, _documentTypeRepository.Object, _fundingSourceRepository.Object,
                                                                        null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, _documentTypeRepository.Object, _fundingSourceRepository.Object,
                                                                        _applicationRepository.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, _documentTypeRepository.Object, _fundingSourceRepository.Object,
                                                                        _applicationRepository.Object, _uiHelperService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                                                        _unitRepository.Object, _unitHouseholdRepository.Object, _amenityRepository.Object,
                                                                        _accessibilityRepository.Object, _declarationRepository.Object, _disclosureRepository.Object,
                                                                        _documentRepository.Object, _documentTypeRepository.Object, _fundingSourceRepository.Object,
                                                                        _applicationRepository.Object, _uiHelperService.Object, _metadataService.Object, null));

        // Not Null
        var actual = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetDataTests()
    {
        // Exception
        var listingRepository = new Mock<IListingRepository>();
        listingRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetData(It.IsAny<string>(), It.IsAny<string>()));

        // Null listings
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Listings);

        // No listings
        listingRepository.Setup(s => s.GetAll()).ReturnsAsync([]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Listings);

        // With listings
        listingRepository.Setup(s => s.GetAll()).ReturnsAsync([new() { ListingId = 1, ListingTypeCd = "SALE" }, new() { ListingId = 2, ListingTypeCd = "RENTAL" }]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Listings);
        Assert.Equal(2, actual.Listings.Count());
        Assert.Single(actual.RentalListings);
        Assert.Single(actual.SaleListings);
    }

    [Fact]
    public async void GetOneTests()
    {
        // Exception
        var listingRepository = new Mock<IListingRepository>();
        listingRepository.Setup(s => s.GetOne(It.IsAny<Listing>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Null
        listingRepository.Setup(s => s.GetOne(It.IsAny<Listing>())).ReturnsAsync(new Listing());
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);

        // With Application
        var applicationRepository = new Mock<IHousingApplicationRepository>();
        applicationRepository.Setup(s => s.GetAllByListing(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                                .ReturnsAsync(new List<HousingApplication>() { new() { ApplicationId = 1 } });
        listingRepository.Setup(s => s.GetOne(It.IsAny<Listing>())).ReturnsAsync(new Listing());
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), "USERNAME");
        Assert.NotNull(actual);
        Assert.NotNull(actual.Application);
        Assert.Equal(1, actual.Application.ApplicationId);
    }

    [Fact]
    public async void SearchTests()
    {
        // Exception
        var listingRepository = new Mock<IListingRepository>();
        listingRepository.Setup(s => s.Search(It.IsAny<ListingSearchParameters>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ListingSearchViewModel>()));

        // Null listings
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.Search(It.IsAny<string>(), It.IsAny<string>(), new ListingSearchViewModel());
        Assert.NotNull(actual);
        Assert.Empty(actual.Listings);

        // No listings
        listingRepository.Setup(s => s.Search(It.IsAny<ListingSearchParameters>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.Search(It.IsAny<string>(), It.IsAny<string>(), new ListingSearchViewModel());
        Assert.NotNull(actual);
        Assert.Empty(actual.Listings);

        // With listings
        listingRepository.Setup(s => s.Search(It.IsAny<ListingSearchParameters>()))
            .ReturnsAsync([new() { ListingId = 1, ListingTypeCd = "SALE" }, new() { ListingId = 2, ListingTypeCd = "RENTAL" }]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.Search(It.IsAny<string>(), It.IsAny<string>(), new ListingSearchViewModel());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Listings);
        Assert.Equal(2, actual.Listings.Count());
        Assert.Single(actual.RentalListings);
        Assert.Single(actual.SaleListings);
    }

    [Fact]
    public async void GetImagesTests()
    {
        // Exception
        var imageRepository = new Mock<IListingImageRepository>();
        imageRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetImages(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null images
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetImages(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No images
        imageRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetImages(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With images
        imageRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { ImageId = 1, Contents = "ABCD" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetImages(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetUnitsTests()
    {
        // Exception
        var unitRepository = new Mock<IListingUnitRepository>();
        unitRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetUnits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null units
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetUnits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No units
        unitRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetUnits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With units
        unitRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { UnitId = 1, UnitTypeCd = "1BED" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetUnits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetUnitHouseholdsTests()
    {
        // Exception
        var unitHouseholdRepository = new Mock<IListingUnitHouseholdRepository>();
        unitHouseholdRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetHouseholds(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null unit households
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetHouseholds(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No unit households
        unitHouseholdRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetHouseholds(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With unit households
        unitHouseholdRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { UnitId = 1, HouseholdSize = 2 }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetHouseholds(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetAmenitiesTests()
    {
        // Exception
        var amenityRepository = new Mock<IListingAmenityRepository>();
        amenityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetAmenities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null amenities
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetAmenities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No amenities
        amenityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetAmenities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With amenities
        amenityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { AmenityId = 1, Name = "NAME" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetAmenities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetAccessiblitiesTests()
    {
        // Exception
        var accessibilityRepository = new Mock<IListingAccessibilityRepository>();
        accessibilityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetAccessibilities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null accessibilities
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetAccessibilities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No accessibilities
        accessibilityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetAccessibilities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With accessibilities
        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>())).Returns(new Dictionary<string, string>() { { "Code", "Description" } });
        accessibilityRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { Active = true, Code = "CODE" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetAccessibilities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetDeclarationsTests()
    {
        // Exception
        var declarationRepository = new Mock<IListingDeclarationRepository>();
        declarationRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetDeclarations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null declarations
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetDeclarations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No declarations
        declarationRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetDeclarations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With declarations
        declarationRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { DeclarationId = 1, Text = "TEXT" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetDeclarations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetDisclosuresTests()
    {
        // Exception
        var disclosureRepository = new Mock<IListingDisclosureRepository>();
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetDisclosures(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null declarations
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetDisclosures(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // No declarations
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetDisclosures(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Empty(actual);

        // With declarations
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { DisclosureId = 1, Text = "TEXT" }]);
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetDisclosures(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async void GetPrintableFormTests()
    {
        // Exception
        var listingRepository = new Mock<IListingRepository>();
        listingRepository.Setup(s => s.GetOne(It.IsAny<Listing>())).ThrowsAsync(new Exception() { });
        var service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        // Null
        service = new ListingService(_logger.Object, _listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        var actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Null
        listingRepository.Setup(s => s.GetOne(It.IsAny<Listing>())).ReturnsAsync(new Listing());
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("/images/listings-default-image.png", actual.ImageContents);
        Assert.Null(actual.PetDisclosure);

        // With images
        var imageRepository = new Mock<IListingImageRepository>();
        imageRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { ImageId = 1, Contents = "IMAGE1" }, new() { ImageId = 2, Contents = "IMAGE2" }]);
        service = new ListingService(_logger.Object, listingRepository.Object, imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("IMAGE1", actual.ImageContents);

        // With images
        imageRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { ImageId = 1, Contents = "IMAGE1" }, new() { ImageId = 2, Contents = "IMAGE2", IsPrimary = true }]);
        service = new ListingService(_logger.Object, listingRepository.Object, imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, _disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("IMAGE2", actual.ImageContents);

        // With No Disclosures
        var disclosureRepository = new Mock<IListingDisclosureRepository>();
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Null(actual.PetDisclosure);

        // With Pet Disclosure (Yes)
        disclosureRepository = new Mock<IListingDisclosureRepository>();
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { Code = "DISCPETYES", Text = "Pets Allowed" }]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("Pets Allowed", actual.PetDisclosure);

        // With Pet Disclosure (No)
        disclosureRepository = new Mock<IListingDisclosureRepository>();
        disclosureRepository.Setup(s => s.GetAll(It.IsAny<int>())).ReturnsAsync([new() { Code = "DISCPETNO", Text = "Pets Not Allowed" }]);
        service = new ListingService(_logger.Object, listingRepository.Object, _imageRepository.Object,
                                            _unitRepository.Object, _unitHouseholdRepository.Object,
                                            _amenityRepository.Object, _accessibilityRepository.Object,
                                            _declarationRepository.Object, disclosureRepository.Object,
                                            _documentRepository.Object, _documentTypeRepository.Object,
                                            _fundingSourceRepository.Object, _applicationRepository.Object,
                                            _uiHelperService.Object, _metadataService.Object, _amortizationsService.Object);
        actual = await service.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("Pets Not Allowed", actual.PetDisclosure);
    }
}