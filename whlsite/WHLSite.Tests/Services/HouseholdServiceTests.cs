using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Services;

public class HouseholdServiceTests
{
    private readonly Mock<ILogger<HouseholdService>> _logger = new();
    private readonly Mock<IHouseholdRepository> _householdRepository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IPhoneService> _phoneService = new();
    private readonly Mock<IUiHelperService> _uiHelperService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(_logger.Object, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(_logger.Object, _householdRepository.Object, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, null));

        // Not Null
        var actual = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        Assert.NotNull(actual);
    }

    //[Fact]
    public void IsIncompleteHouseholdTests()
    {
        // Null test
        Household household = null;
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.False(actual);

        // Street test
        household = new Household();
        service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.True(actual);

        // City test
        household.PhysicalStreetLine1 = "STREET1";
        service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.True(actual);

        // State test
        household.PhysicalCity = "CITY";
        service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.True(actual);

        // Zip Code test
        household.PhysicalStateCd = "STATE";
        service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.True(actual);

        // Valid
        household.PhysicalZipCode = "ZIPCD";
        service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = service.IsIncompleteHousehold(It.IsAny<string>(), It.IsAny<string>(), household);
        Assert.False(actual);
    }

    [Fact]
    public async void GetOneTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Single member household test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { AddressInd = true, PhysicalStreetLine1 = "STREET1" });
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([new() { Username = "USERNAME", RelationTypeCd = "SELF", MemberId = 0 }]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.AddressInd);
        Assert.Equal("STREET1", actual.PhysicalStreetLine1);
        Assert.Single(actual.Members);

        // Multi-member household test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { AddressInd = true, PhysicalStreetLine1 = "STREET1" });
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([new() { Username = "USERNAME", RelationTypeCd = "SELF", MemberId = 0 }, new() { Username = "MEMBER1", RelationTypeCd = "SPOUSE", MemberId = 1, Last4SSN = "1234" }]);
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetRelationTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "SPOUSE", "Spouse" }, { "OTHER", "Other" } });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.AddressInd);
        Assert.Equal("STREET1", actual.PhysicalStreetLine1);
        Assert.Equal(2, actual.Members.Count);

        // Household with vouchers test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { VoucherInd = true, VoucherCds = "A,OTHER", VoucherOther = "O", VoucherAdminName = "ADMIN" });
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([]);
        householdRepository.Setup(s => s.GetAccounts(It.IsAny<long>())).ReturnsAsync([]);
        metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetVoucherTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "A", "Voucher A" }, { "OTHER", "Other" } });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.VoucherInd);
        Assert.Equal("Voucher A, Other: O", actual.DisplayVouchers);
        Assert.Equal("ADMIN", actual.VoucherAdminName);

        // Household with accounts test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { AddressInd = true, PhysicalStreetLine1 = "STREET1" });
        householdRepository.Setup(s => s.GetAccounts(It.IsAny<long>())).ReturnsAsync([new() { AccountNumber = "1234" }]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Single(actual.Accounts);
        Assert.Equal("1234", actual.Accounts.First().AccountNumber);
    }

    [Fact]
    public async void GetForAddressInfoEditTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetForAddressInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { AddressInd = true, PhysicalStreetLine1 = "STREET1" });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForAddressInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.AddressInd);
        Assert.Equal("STREET1", actual.PhysicalStreetLine1);
    }

    [Fact]
    public async void SaveAddressInfoTests()
    {
        // Null test
        EditableAddressInfoViewModel model = null;
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H101", actual);

        // Household not found test
        model = new EditableAddressInfoViewModel();
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H001", actual);

        // No address test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.UpdateAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Missing Physical Street 1
        model.AddressInd = true;
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H102", actual);

        // Missing Physical City
        model.PhysicalStreetLine1 = "STREET1";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H103", actual);

        // Missing Physical State
        model.PhysicalCity = "CITY";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H104", actual);

        // Missing Physical Zip Code
        model.PhysicalStateCd = "STATE";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H105", actual);

        // Has Physical Address test
        model.PhysicalZipCode = "ZIPCD";
        householdRepository.Setup(s => s.UpdateAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Different Mailing Address test, missing Mailing Street 1
        model.DifferentMailingAddressInd = true;
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H112", actual);

        // Different Mailing Address test, missing Mailing City
        model.MailingStreetLine1 = "MSTREET1";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H113", actual);

        // Different Mailing Address test, missing Mailing State
        model.MailingCity = "MCITY";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H114", actual);

        // Different Mailing Address test, missing Mailing Zip Code
        model.MailingStateCd = "MSTATE";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H115", actual);

        // Save failure test
        model.MailingZipCode = "MZIPCD";
        householdRepository.Setup(s => s.UpdateAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H004", actual);

        // Save success test
        householdRepository.Setup(s => s.UpdateAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetForVoucherInfoEditTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetForVoucherInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { VoucherInd = true, VoucherCds = "A", VoucherAdminName = "ADMIN" });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForVoucherInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.VoucherInd);
        Assert.Equal("A", actual.VoucherCds);
        Assert.Equal("ADMIN", actual.VoucherAdminName);
    }

    [Fact]
    public async void SaveVoucherInfoTests()
    {
        // Null test
        EditableVoucherInfoViewModel model = null;
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H101", actual);

        // Household not found test
        model = new EditableVoucherInfoViewModel();
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H001", actual);

        // Missing voucher codes test
        model.VoucherInd = true;
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H211", actual);

        // Invalid voucher code test
        model.VoucherCds = "A";
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H212", actual);

        // Missing other voucher test
        model.VoucherCds = "OTHER";
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetVoucherTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "A", "Voucher A" }, { "OTHER", "Other" } });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H213", actual);

        // Missing voucher admin test
        model.VoucherCds = "A";
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetVoucherTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "A", "Voucher A" }, { "OTHER", "Other" } });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H214", actual);

        // Save failure test
        model.VoucherInd = false;
        householdRepository.Setup(s => s.UpdateVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H204", actual);

        // Save success test
        householdRepository.Setup(s => s.UpdateVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetForLiveInAideInfoEditTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetForLiveInAideInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { LiveInAideInd = true });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForLiveInAideInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.True(actual.LiveInAideInd);
    }

    [Fact]
    public async void SaveLiveInAideInfoTests()
    {
        // Null test
        EditableLiveInAideInfoViewModel model = null;
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H101", actual);

        // Household not found test
        model = new EditableLiveInAideInfoViewModel();
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H001", actual);

        // Save failure test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.UpdateLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H304", actual);

        // Save success test
        householdRepository.Setup(s => s.UpdateLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Household>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void AssignMetadataForMemberEditTests()
    {
        var model = new EditableHouseholdMemberViewModel();
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetGenderTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "MALE", "Male" } });
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.NotEmpty(model.GenderTypes);
    }

    [Fact]
    public async void GetForMemberInfoEditTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>());
        Assert.Null(actual);

        // Add member test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.NotNull(actual);
        Assert.Equal(0, actual.MemberId);

        // Edit member not found test
        householdRepository.Setup(s => s.GetMember(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((HouseholdMember)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Null(actual);

        // Edit member found test
        householdRepository.Setup(s => s.GetMember(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new HouseholdMember() { MemberId = 1 });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.NotNull(actual);
        Assert.Equal(1, actual.MemberId);
    }

    [Fact]
    public async void SaveMemberInfoTests()
    {
        // Invalid input test
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>());
        Assert.Equal("H101", actual);

        // Household not found test
        var model = new EditableHouseholdMemberViewModel();
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H001", actual);

        // Setup
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());

        // Setup
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetRelationTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "SPOUSE", "Spouse" }, { "OTHER", "Other" } });

        // Missing relation type
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H411", actual);

        // Missing other relation type
        model.RelationTypeCd = "OTHER";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H412", actual);

        // Missing first name
        model.RelationTypeCd = "SPOUSE";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P102", actual);

        // Missing last name
        model.FirstName = "FIRST";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P103", actual);

        // Missing SSN
        model.LastName = "LAST";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P104", actual);

        // Missing date of birth
        model.Last4SSN = "1234";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P105", actual);

        // Invalid date of birth
        model.DateOfBirth = "1899-12-31";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P105", actual);

        // Setup
        metadataService.Setup(s => s.GetIdTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "STATEDL", "State Drivers License" } });

        // Invalid ID type
        model.DateOfBirth = "1980-01-01";
        model.IdTypeCd = "ID";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P106", actual);

        // Invalid ID Value
        model.IdTypeCd = "STATEDL";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P107", actual);

        // Invalid ID Issue Date
        model.IdTypeValue = "IDVALUE";
        model.IdIssueDate = "1899-12-31";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P108", actual);

        // Setup
        model.IdIssueDate = "1996-12-31";
        metadataService.Setup(s => s.GetGenderTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "MALE", "Male" }, { "NOANS", "Choose not to answer" } });
        metadataService.Setup(s => s.GetRaceTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "WHITE", "White" }, { "NOANS", "Choose not to answer" } });
        metadataService.Setup(s => s.GetEthnicityTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "HISPANIC", "Hispanic" }, { "NOANS", "Choose not to answer" } });

        // Invalid Gender Type
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P109", actual);

        // Invalid Race Type
        model.GenderCd = "NOANS";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P110", actual);

        // Invalid Ethnicity Type
        model.RaceCd = "NOANS";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P111", actual);

        // Invalid phone number
        model.EthnicityCd = "NOANS";
        model.PhoneNumber = "1234567890";
        var phoneService = new Mock<IPhoneService>();
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P211", actual);

        // Setup
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "CELL", "Cell" }, { "OTHER", "Other" } });

        // Invalid phone type
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P212", actual);

        // Invalid alternate phone number
        model.PhoneNumber = null;
        model.PhoneNumberTypeCd = null;
        model.AltPhoneNumber = "1234567890";
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P213", actual);

        // Invalid alternate phone type
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P214", actual);

        // Invalid email address
        model.AltPhoneNumberTypeCd = "CELL";
        model.EmailAddress = "USER@UNIT.TEST";
        var emailService = new Mock<IEmailService>();
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P215", actual);

        // Invalid alternate email address
        model.EmailAddress = null;
        model.AltEmailAddress = "USER@UNIT.TEST";
        service = new HouseholdService(_logger.Object, householdRepository.Object, emailService.Object, metadataService.Object, phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P216", actual);

        // Save Profile failure
        model.PhoneNumber = null;
        model.AltPhoneNumber = null;
        model.EmailAddress = null;
        model.AltEmailAddress = null;
        householdRepository.Setup(s => s.SaveMemberProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdMember>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H403", actual);

        // Save success
        householdRepository.Setup(s => s.SaveMemberProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdMember>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Duplicate for add test
        model = new EditableHouseholdMemberViewModel() { MemberId = 0, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", DateOfBirth = "1990-01-01", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS" };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([ new() { MemberId = 1, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", DateOfBirth = new DateTime(1990, 1, 1), GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS" } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H402", actual);

        // Member not found test
        model = new EditableHouseholdMemberViewModel() { MemberId = 1, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", DateOfBirth = "1990-01-01", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS" };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([ new() { MemberId = 2, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS" } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H401", actual);

        // Duplicate for edit test
        model = new EditableHouseholdMemberViewModel() { MemberId = 1, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", DateOfBirth = "1990-01-01", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS" };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetMembers(It.IsAny<long>())).ReturnsAsync([ new() { MemberId = 1, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS", Last4SSN = "2345", DateOfBirth = new DateTime(1990, 1, 1) }, new() { MemberId = 2, RelationTypeCd = "SPOUSE", FirstName = "FIRST", LastName = "LAST", GenderCd = "NOANS", RaceCd = "NOANS", EthnicityCd = "NOANS", Last4SSN = "1234", DateOfBirth = new DateTime(1990, 1, 1) } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H402", actual);
    }

    [Fact]
    public async void DeleteMemberInfoTests()
    {
        // Invalid input test
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>());
        Assert.Equal("H101", actual);

        // Household not found test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H001", actual);

        // Account not found test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetMember(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((HouseholdMember)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H401", actual);

        // Delete failure
        householdRepository.Setup(s => s.GetMember(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new HouseholdMember());
        householdRepository.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdMember>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H405", actual);

        // Delete failure
        householdRepository.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdMember>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void AssignMetadataForAccountEditTests()
    {
        var model = new EditableHouseholdAccountViewModel();
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetAccountTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "CHECKING", "Checking" } });
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.NotEmpty(model.AccountTypes);
    }

    [Fact]
    public async void GetForAccountInfoEditTests()
    {
        // Null test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        var service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>());
        Assert.Null(actual);

        // Add member test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household() { });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.NotNull(actual);
        Assert.Equal(0, actual.AccountId);

        // Edit member not found test
        householdRepository.Setup(s => s.GetAccount(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((HouseholdAccount)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Null(actual);

        // Edit member found test
        householdRepository.Setup(s => s.GetAccount(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new HouseholdAccount() { AccountId = 1 });
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AccountId);
    }

    [Fact]
    public async void SaveAccountInfoTests()
    {
        // Invalid input test
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>());
        Assert.Equal("H101", actual);

        // Household not found test
        var model = new EditableHouseholdAccountViewModel();
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H001", actual);

        // Setup
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());

        // Missing account type
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H512", actual);

        // Setup
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetAccountTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "CHECKING", "Checking" }, { "OTHER", "Other" } });

        // Invalid account type
        model.AccountTypeCd = "ABCD";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H513", actual);

        // Missing other account type
        model.AccountTypeCd = "OTHER";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H514", actual);

        // Missing account number
        model.AccountTypeCd = "CHECKING";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H511", actual);

        // Missing institution name
        model.AccountNumber = "1234";
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H515", actual);

        // Save failure
        model.InstitutionName = "NAME";
        householdRepository.Setup(s => s.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdAccount>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H504", actual);

        // Save success
        householdRepository.Setup(s => s.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdAccount>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Duplicate for add test
        model = new EditableHouseholdAccountViewModel() { AccountId = 0, AccountNumber = "1234", AccountTypeCd = "CHECKING", InstitutionName = "NAME", PrimaryHolderMemberId = 1 };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetAccounts(It.IsAny<long>())).ReturnsAsync([ new() { AccountId = 1, AccountNumber = "1234", AccountTypeCd = "CHECKING", InstitutionName = "NAME", PrimaryHolderMemberId = 1 } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H502", actual);

        // Account not found test
        model = new EditableHouseholdAccountViewModel() { AccountId = 1, AccountNumber = "1234", AccountTypeCd = "CHECKING", InstitutionName = "NAME" };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetAccounts(It.IsAny<long>())).ReturnsAsync([ new() { AccountId = 2, AccountNumber = "1234", AccountTypeCd = "CHECKING" } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H501", actual);

        // Duplicate for edit test
        model = new EditableHouseholdAccountViewModel() { AccountId = 2, AccountNumber = "1234", AccountTypeCd = "CHECKING", InstitutionName = "NAME", PrimaryHolderMemberId = 1 };
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetAccounts(It.IsAny<long>())).ReturnsAsync([ new() { AccountId = 1, AccountNumber = "1234", AccountTypeCd = "CHECKING", InstitutionName = "NAME", PrimaryHolderMemberId = 1 }, new() { AccountId = 2, AccountNumber = "1234", AccountTypeCd = "CHECKING" } ]);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("H502", actual);
    }

    [Fact]
    public async void DeleteAccountInfoTests()
    {
        // Invalid input test
        var service = new HouseholdService(_logger.Object, _householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        var actual = await service.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>());
        Assert.Equal("H101", actual);

        // Household not found test
        var householdRepository = new Mock<IHouseholdRepository>();
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((Household)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H001", actual);

        // Account not found test
        householdRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new Household());
        householdRepository.Setup(s => s.GetAccount(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((HouseholdAccount)null);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H501", actual);

        // Delete failure
        householdRepository.Setup(s => s.GetAccount(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new HouseholdAccount());
        householdRepository.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdAccount>())).ReturnsAsync(false);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("H505", actual);

        // Delete failure
        householdRepository.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HouseholdAccount>())).ReturnsAsync(true);
        service = new HouseholdService(_logger.Object, householdRepository.Object, _emailService.Object, _metadataService.Object, _phoneService.Object, _uiHelperService.Object);
        actual = await service.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("", actual);
    }
}