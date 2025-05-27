using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Services;

public class ProfileServiceTests
{
    private readonly Mock<ILogger<ProfileService>> _logger = new();
    private readonly Mock<IProfileRepository> _profileRepository = new();
    private readonly Mock<IMasterConfigService> _configService = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IKeyService> _keyService = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IPhoneService> _phoneService = new();
    private readonly Mock<IUiHelperService> _uiHelperService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ProfileService(null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, null, null, null, null,
                                                                        null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object,
                                                                        null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object,
                                                                        _emailService.Object, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object,
                                                                        _emailService.Object, _keyService.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object,
                                                                        _emailService.Object, _keyService.Object, _metadataService.Object,
                                                                        null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object,
                                                                        _emailService.Object, _keyService.Object, _metadataService.Object,
                                                                        _phoneService.Object, null));

        // Not Null
        var actual = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void IsIncompleteProfileTests()
    {
        // Null test
        ProfileViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.False(actual);

        // Missing First Name test
        model = new ProfileViewModel();
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing Last Name test
        model.FirstName = "FIRST";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing SSN test
        model.LastName = "LAST";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing Date of Birth test
        model.Last4SSN = "1234";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing ID test
        model.DateOfBirth = DateTime.Today.AddYears(-20);
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing Gender test
        model.IdTypeCd = "DL";
        model.IdTypeDescription = "Driver's License";
        model.IdTypeValue = "NY 1234";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing Race test
        model.GenderCd = "MALE";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);

        // Missing Ethnicity test
        model.RaceCd = "WHITE";
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = service.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.True(actual);
    }

    [Fact]
    public async void GetOneTests()
    {
        // Null test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Not null test
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { Username = "USERNAME" });
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("USERNAME", actual.Username);
    }

    [Fact]
    public async void AssignMetadataForEditableProfileTests()
    {
        // Null test
        EditableProfileViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.Null(model);

        // Not null test
        model = new EditableProfileViewModel();
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetIdTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "STATEDL", "Drivers License" }, { "OTHER", "Other" } });
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.NotNull(model);
        Assert.NotEmpty(model.IdTypes);
    }

    [Fact]
    public async void GetForProfileInfoEditTests()
    {
        // Null test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetForProfileInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { FirstName = "FIRST", LastName = "LAST", Last4SSN = "1234", DateOfBirth = DateTime.Today });
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetGenderTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "MALE", "Male" }, { "NOANS", "Choose not to respond" } });
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetForProfileInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("FIRST", actual.FirstName);
        Assert.Equal("LAST", actual.LastName);
        Assert.Equal("1234", actual.Last4SSN);
        Assert.Equal($"{DateTime.Today:yyyy-MM-dd}", actual.DateOfBirth);
        Assert.NotEmpty(actual.GenderTypes);
    }

    [Fact]
    public async void SaveProfileInfoTests()
    {
        // Null test
        EditableProfileViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P101", actual);

        // Profile not found test
        model = new EditableProfileViewModel();
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P001", actual);

        // Setup
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());

        // Missing first name test
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P102", actual);

        // Invalid first name test
        model.FirstName = "123FIRST";
        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.IsValidNameWithSpecialCharacters("123FIRST")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P1021", actual);

        // Invalid middle name test
        model.FirstName = "FIRST";
        model.MiddleName = "123MID";
        keyService.Setup(s => s.IsValidNameWithSpecialCharacters("FIRST")).Returns(true);
        keyService.Setup(s => s.IsValidName("123MID")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P1022", actual);

        // Missing last name test
        model.FirstName = "FIRST";
        model.MiddleName = null;
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P103", actual);

        // Missing last name test
        model.FirstName = "FIRST";
        model.MiddleName = null;
        model.LastName = "123LAST";
        keyService.Setup(s => s.IsValidNameWithSpecialCharacters("123LAST")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P1031", actual);

        // Missing SSN test
        model.LastName = "LAST";
        keyService.Setup(s => s.IsValidNameWithSpecialCharacters(It.IsAny<string>())).Returns(true);
        keyService.Setup(s => s.IsValidName(It.IsAny<string>())).Returns(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P104", actual);

        // Missing Date of Borth
        model.Last4SSN = "1234";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P105", actual);

        // Invalid Date of Birth test
        model.DateOfBirth = "1899-12-31";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P105", actual);

        // Setup
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetIdTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "STATEDL", "State Drivers License" } });

        // Invalid Id Type test
        model.DateOfBirth = "1990-01-01";
        model.IdTypeCd = "ID";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P106", actual);

        // Invalid ID Value
        model.IdTypeCd = "STATEDL";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P107", actual);

        // Invalid ID Issue Date
        model.IdTypeValue = "IDVALUE";
        model.IdIssueDate = "1899-12-31";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P108", actual);

        // Setup
        model.IdIssueDate = "1996-12-31";
        metadataService.Setup(s => s.GetGenderTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "MALE", "Male" }, { "NOANS", "Choose not to answer" } });
        metadataService.Setup(s => s.GetRaceTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "WHITE", "White" }, { "NOANS", "Choose not to answer" } });
        metadataService.Setup(s => s.GetEthnicityTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "HISPANIC", "Hispanic" }, { "NOANS", "Choose not to answer" } });

        // Invalid Gender Type
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P109", actual);

        // Invalid Race Type
        model.GenderCd = "NOANS";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P110", actual);

        // Invalid Ethnicity Type
        model.RaceCd = "NOANS";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P111", actual);

        // Save Profile failure
        model.EthnicityCd = "NOANS";
        profileRepository.Setup(s => s.UpdateProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P004", actual);
    }

    [Fact]
    public async void GetForContactInfoEditTests()
    {
        // Null test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetForContactInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { PhoneNumber = "1234567890", PhoneNumberTypeCd = "CELL" });
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "CELL", "Cell" }, { "OTHER", "Other" } });
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetForContactInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("1234567890", actual.PhoneNumber);
        Assert.Equal("CELL", actual.PhoneNumberTypeCd);
        Assert.NotEmpty(actual.PhoneNumberTypes);
    }

    [Fact]
    public async void SaveContactInfoTests()
    {
        // Null test
        EditableProfileViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P101", actual);

        // Profile not found test
        model = new EditableProfileViewModel();
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P001", actual);

        // Setup
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());

        // Invalid phone number
        model.PhoneNumber = "1234567890";
        var phoneService = new Mock<IPhoneService>();
        phoneService.Setup(s => s.IsValidPhoneNumber("1234567890")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P211", actual);

        // Setup
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "CELL", "Cell" }, { "OTHER", "Other" } });

        // Invalid phone type
        phoneService.Setup(s => s.IsValidPhoneNumber("1234567890")).Returns(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P212", actual);

        // Invalid alternate phone number
        model.PhoneNumberTypeCd = "CELL";
        model.AltPhoneNumber = "1234567891";
        phoneService.Setup(s => s.IsValidPhoneNumber("1234567891")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P213", actual);

        // Invalid alternate phone type
        phoneService.Setup(s => s.IsValidPhoneNumber("1234567891")).Returns(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P214", actual);

        // Invalid email address
        model.AltPhoneNumber = null;
        model.AltPhoneNumberTypeCd = null;
        model.EmailAddress = "USER@UNIT.TEST";
        var emailService = new Mock<IEmailService>();
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P215", actual);

        // Invalid alternate email address
        model.AltPhoneNumber = null;
        model.AltPhoneNumberTypeCd = null;
        model.EmailAddress = "USER@UNIT.TEST";
        model.AltEmailAddress = "USER1@UNIT.TEST";
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), "USER@UNIT.TEST")).Returns(true);
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), "USER1@UNIT.TEST")).Returns(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P216", actual);

        // Same primary and alternate email address
        model.EmailAddress = "USER@UNIT.TEST";
        model.AltEmailAddress = "USER@UNIT.TEST";
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), model.Username, model);
        Assert.Equal("P217", actual);

        // Save failure
        model = new EditableProfileViewModel() { EmailAddress = "USER@UNIT.TEST", PhoneNumber = "1234567890", PhoneNumberTypeCd = "CELL" };
        profileRepository.Setup(s => s.UpdateContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P004", actual);

        // Save success
        profileRepository.Setup(s => s.UpdateContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, emailService.Object,
                                            _keyService.Object, metadataService.Object, phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void AssignMetadataForEditablePreferencesInfoTests()
    {
        // Null test
        EditablePreferencesViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.Null(model);

        // Not null test
        model = new EditablePreferencesViewModel();
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetLanguages(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "ENGLISH", "English" }, { "OTHER", "Other" } });
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        await service.AssignMetadata(model);
        Assert.NotNull(model);
        Assert.NotEmpty(model.LanguageTypes);
    }

    [Fact]
    public async void GetForPreferencesInfoEditTests()
    {
        // Null test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetForPreferencesInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { LanguagePreferenceCd = "ENGLISH" });
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetLanguages(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "ENGLISH", "English" }, { "OTHER", "Other" } });
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetForPreferencesInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("ENGLISH", actual.LanguagePreferenceCd);
        Assert.NotEmpty(actual.LanguageTypes);
    }

    [Fact]
    public async void SavePreferencesInfoTests()
    {
        // Null test
        EditablePreferencesViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P101", actual);

        // Profile not found test
        model = new EditablePreferencesViewModel();
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P001", actual);

        // Setup
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetLanguages(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "ENGLISH", "English" }, { "OTHER", "Other" } });
        metadataService.Setup(s => s.GetListingTypes(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>() { { "RENTAL", "Rentals" }, { "SALES", "Sales" } });

        // Invalid language type
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P311", actual);

        // Missing language type other
        model.LanguagePreferenceCd = "OTHER";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P312", actual);

        // Invalid listing type
        model.LanguagePreferenceCd = "ENGLISH";
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P313", actual);

        // Save failure
        model = new EditablePreferencesViewModel() { LanguagePreferenceCd = "ENGLISH", ListingPreferenceCd = "RENTAL" };
        profileRepository.Setup(s => s.UpdatePreferences(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P304", actual);

        // Save success
        profileRepository.Setup(s => s.UpdatePreferences(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetForNetWorthInfoEditTests()
    {
        // Null test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetForNetWorthInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Valid test
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { IncomeValueAmt = 1000L });
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetForNetWorthInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(1000L, actual.IncomeValueAmt);
    }

    [Fact]
    public async void SaveNetWorthInfoTests()
    {
        // Null test
        EditableNetWorthViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P101", actual);

        // Profile not found test
        model = new EditableNetWorthViewModel();
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P001", actual);

        // Setup
        profileRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());

        // Save failure
        model = new EditableNetWorthViewModel() { IncomeValueAmt = 1000L, AssetValueAmt = 2000L };
        profileRepository.Setup(s => s.UpdateNetWorth(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("P404", actual);

        // Save success
        profileRepository.Setup(s => s.UpdateNetWorth(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetNotificationsTest()
    {
        // Empty test
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Notifications);

        // Non-empty test
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync([new() { NotificationId = 1, Subject = "SUBJECT" }]);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Notifications);
    }

    [Fact]
    public async void UpdateNotificationTests()
    {
        // Null test
        EditableUserNotificationViewModel model = null;
        var service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        var actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N101", actual);

        // Invalid notification ID test
        model = new EditableUserNotificationViewModel();
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N102", actual);

        // Invalid action test
        model.NotificationId = -999;
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N103", actual);

        // Invalid action test
        model.NotificationId = 1;
        service = new ProfileService(_logger.Object, _profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N103", actual);

        // Update failure test
        model = new EditableUserNotificationViewModel() { NotificationId = 1, Action = "R" };
        var profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserNotification>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N004", actual);

        // Update success test
        model = new EditableUserNotificationViewModel() { NotificationId = 1, Action = "R" };
        profileRepository.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserNotification>())).ReturnsAsync(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Delete failure test
        model = new EditableUserNotificationViewModel() { NotificationId = 1, Action = "D" };
        profileRepository = new Mock<IProfileRepository>();
        profileRepository.Setup(s => s.DeleteNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserNotification>())).ReturnsAsync(false);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("N005", actual);

        // Delete success test
        model = new EditableUserNotificationViewModel() { NotificationId = 1, Action = "D" };
        profileRepository.Setup(s => s.DeleteNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserNotification>())).ReturnsAsync(true);
        service = new ProfileService(_logger.Object, profileRepository.Object, _configService.Object, _emailService.Object,
                                            _keyService.Object, _metadataService.Object, _phoneService.Object,
                                            _uiHelperService.Object);
        actual = await service.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }
}