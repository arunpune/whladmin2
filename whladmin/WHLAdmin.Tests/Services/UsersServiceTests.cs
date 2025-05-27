using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class UsersServiceTests()
{
    private readonly Mock<ILogger<UsersService>> _logger = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IAdminUserRepository> _userRepository = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IKeyService> _keyService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new UsersService(null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersService(_logger.Object, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, null));

        // Not Null
        var actual = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var userRepositoryEmpty = new Mock<IAdminUserRepository>();
        userRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var userRepositoryNonEmpty = new Mock<IAdminUserRepository>();
        userRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
            }
        ]);

        // Empty
        var service = new UsersService(_logger.Object, _configuration.Object, userRepositoryEmpty.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Users);

        // Not Empty
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNonEmpty.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Users);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var userRepositoryEmpty = new Mock<IAdminUserRepository>();
        userRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var userRepositoryNonEmpty = new Mock<IAdminUserRepository>();
        userRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
            }
        ]);

        // Empty
        var service = new UsersService(_logger.Object, _configuration.Object, userRepositoryEmpty.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNonEmpty.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var userRepositoryNull = new Mock<IAdminUserRepository>();
        userRepositoryNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryNotNull = new Mock<IAdminUserRepository>();
        userRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });

        // Null
        var service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetOne(It.IsAny<string>());
        Assert.Null(actual);

        // Not Empty
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetOne(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
        Assert.Equal("NAME", actual.DisplayName);
        Assert.Equal("ROLECD", actual.RoleCd);
        Assert.Equal("ORGCD", actual.OrganizationCd);
    }

    [Fact]
    public async Task SendOtpTests()
    {
        // Setup
        var userRepositoryNull = new Mock<IAdminUserRepository>();
        userRepositoryNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryNotNull = new Mock<IAdminUserRepository>();
        userRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = false
        });

        // Null
        var service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.SendOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.False(actual);

        // Inactive User
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.SendOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.False(actual);

        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.GetOtp(It.IsAny<int>())).Returns("OTP");

        // Active User, SetOtp failure
        userRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        userRepositoryNotNull.Setup(s => s.SetOtp(It.IsAny<string>(), "EMAIL@UNIT.TST", "OTP")).ReturnsAsync(false);
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, _emailService.Object, keyService.Object);
        actual = await service.SendOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.False(actual);

        // Active User, SetOtp success, Send email failure
        userRepositoryNotNull.Setup(s => s.SetOtp(It.IsAny<string>(), "EMAIL@UNIT.TST", "OTP")).ReturnsAsync(true);
        var emailService = new Mock<IEmailService>();
        emailService.Setup(s => s.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST", "OTP")).ReturnsAsync(false);
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, emailService.Object, keyService.Object);
        actual = await service.SendOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.False(actual);

        // Active User, SetOtp success, Send email success
        emailService.Setup(s => s.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST", "OTP")).ReturnsAsync(true);
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, emailService.Object, keyService.Object);
        actual = await service.SendOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.True(actual);
    }

    [Fact]
    public async Task GetOneForAddTests()
    {
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Null(actual.EmailAddress);
        Assert.Null(actual.DisplayName);
        Assert.Empty(actual.RoleCd);
        Assert.Empty(actual.OrganizationCd);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var userRepositoryNull = new Mock<IAdminUserRepository>();
        userRepositoryNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryNotNull = new Mock<IAdminUserRepository>();
        userRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });

        // Null
        var service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>());
        Assert.Null(actual);

        // Not Empty
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNotNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
        Assert.Equal("NAME", actual.DisplayName);
        Assert.Equal("ROLECD", actual.RoleCd);
        Assert.Equal("ORGCD", actual.OrganizationCd);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        User user = null;
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        service.Sanitize(user);
        Assert.Null(user);
    }

    [Theory]
    [InlineData(null, null, null, null, null, "", "", "", "", "")]
    [InlineData("", null, null, null, null, "", "", "", "", "")]
    [InlineData(" ", null, null, null, null, "", "", "", "", "")]
    [InlineData("USER", null, null, null, null, "USER", "", "", "", "")]
    [InlineData("USER ", null, null, null, null, "USER", "", "", "", "")]
    [InlineData(" USER", null, null, null, null, "USER", "", "", "", "")]
    [InlineData(" USER ", null, null, null, null, "USER", "", "", "", "")]
    [InlineData("USER", "", null, null, null, "USER", "", "", "", "")]
    [InlineData("USER", " ", null, null, null, "USER", "", "", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", null, null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST ", null, null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", " EMAIL@UNIT.TST", null, null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", " EMAIL@UNIT.TST ", null, null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "", null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", " ", null, null, "USER", "EMAIL@UNIT.TST", "", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", null, null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME ", null, null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", " NAME", null, null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", " NAME ", null, null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "", null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", " ", null, "USER", "EMAIL@UNIT.TST", "NAME", "", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", null, "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD ", null, "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", " ROLECD", null, "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", " ROLECD ", null, "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", " ", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD ", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", " ORGCD", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", " ORGCD ", "USER", "EMAIL@UNIT.TST", "NAME", "ROLECD", "ORGCD")]
    public void SanitizeObjectTests(string userId, string emailAddress, string displayName, string roleCd, string organizationCd,
                                        string expectedUserId, string expectedEmailAddress, string expectedDisplayName, string expectedRoleCd, string expectedOrganizationCd)
    {
        var user = new User() { UserId = userId, EmailAddress = emailAddress, DisplayName = displayName, RoleCd = roleCd, OrganizationCd = organizationCd };
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        service.Sanitize(user);
        Assert.NotNull(user);
        Assert.Equal(expectedUserId, user.UserId);
        Assert.Equal(expectedEmailAddress, user.EmailAddress);
        Assert.Equal(expectedDisplayName, user.DisplayName);
        Assert.Equal(expectedRoleCd, user.RoleCd);
        Assert.Equal(expectedOrganizationCd, user.OrganizationCd);
    }

    [Fact]
    public async Task ValidateNullTest()
    {
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Validate(null, null);
        Assert.Equal("U000", actual);
    }

    [Theory]
    [InlineData(null, null, null, null, null, "U101")]
    [InlineData("", null, null, null, null, "U101")]
    [InlineData(" ", null, null, null, null, "U101")]
    [InlineData("USER", null, null, null, null, "U102")]
    [InlineData("USER", "", null, null, null, "U102")]
    [InlineData("USER", " ", null, null, null, "U102")]
    [InlineData("USER", "EMAIL", null, null, null, "U102")]
    [InlineData("USER", "EMAIL@UNIT.TST", null, null, null, "U103")]
    [InlineData("USER", "EMAIL@UNIT.TST", "", null, null, "U103")]
    [InlineData("USER", "EMAIL@UNIT.TST", " ", null, null, "U103")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", null, null, "U104")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "", null, "U104")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", " ", null, "U104")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORG", null, "U104")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORGCD", null, "U105")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORGCD", "", "U105")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORGCD", " ", "U105")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORGCD", "ROLE", "U105")]
    [InlineData("USER", "EMAIL@UNIT.TST", "NAME", "ORGCD", "ROLECD", "")]
    public async Task ValidateBaseTests(string userId, string emailAddress, string name, string organizationCd, string roleCd, string expectedCode)
    {
        var userToValidate = new User()
        {
            UserId = userId, EmailAddress = emailAddress, DisplayName = name, RoleCd = roleCd, OrganizationCd = organizationCd
        };

        // Setup
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetOrganizations(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ORGCD", "ORGANIZATION" }
        });
        metadataService.Setup(s => s.GetRoles(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ROLECD", "ROLE" }
        });

        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Validate(userToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public async Task ValidateExistenceTests()
    {
        // Setup
        var userToValidate = new User() { UserId = "USER", EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD" };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetOrganizations(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ORGCD", "ORGANIZATION" }
        });
        metadataService.Setup(s => s.GetRoles(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ROLECD", "ROLE" }
        });

        // Existence Check FAIL for UPDATE Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Validate(userToValidate, [], true);
        Assert.Equal("U001", actual);

        // Existence Check SUCCESS for UPDATE Test
        service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Validate(userToValidate, [ userToValidate ], true);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task ValidateDuplicateTests()
    {
        // Setup
        var userToValidate = new User() { UserId = "USER", EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD" };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetOrganizations(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ORGCD", "ORGANIZATION" }
        });
        metadataService.Setup(s => s.GetRoles(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ROLECD", "ROLE" }
        });

        // Duplicate Check FAIL for ADD Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Validate(userToValidate, [ userToValidate ]);
        Assert.Equal("U002", actual);

        // Duplicate Check SUCCESS for ADD Test
        service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Validate(userToValidate, []);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var userToAdd = new EditableUserViewModel()
        {
            UserId = "USER", EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD"
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetOrganizations(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ORGCD", "ORGANIZATION" }
        });
        metadataService.Setup(s => s.GetRoles(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ROLECD", "ROLE" }
        });

        var userRepositoryException = new Mock<IAdminUserRepository>();
        userRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var userRepositoryFailure = new Mock<IAdminUserRepository>();
        userRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        userRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(false);

        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        userRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(true);

        // Null Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("U000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel());
        Assert.Equal("U101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel() { UserId = "" });
        Assert.Equal("U101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel() { UserId = "   " });
        Assert.Equal("U101", actual);

        // Add Exception
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryException.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), userToAdd));

        // Add Failure
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryFailure.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), userToAdd);
        Assert.Equal("U003", actual);

        // Add Success
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), userToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var userToUpdate = new EditableUserViewModel()
        {
            UserId = "USER", EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD"
        };
        var existingUser = new User()
        {
            UserId = "USER", EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD"
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetOrganizations(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ORGCD", "ORGANIZATION" }
        });
        metadataService.Setup(s => s.GetRoles(It.IsAny<bool>())).ReturnsAsync(new Dictionary<string, string>()
        {
            { "ROLECD", "ROLE" }
        });

        var userRepositoryException = new Mock<IAdminUserRepository>();
        userRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var userRepositoryFailure = new Mock<IAdminUserRepository>();
        userRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingUser ]);
        userRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(false);

        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingUser ]);
        userRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(true);

        // Null Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("U000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel());
        Assert.Equal("U101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel() { UserId = "" });
        Assert.Equal("U101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableUserViewModel() { UserId = "   " });
        Assert.Equal("U101", actual);

        // Update Exception
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryException.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), userToUpdate));

        // Update Failure
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryFailure.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), userToUpdate);
        Assert.Equal("U004", actual);

        // Update Success
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), userToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("U000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("U000", actual);

        // Setup
        var userRepositoryNull = new Mock<IAdminUserRepository>();
        userRepositoryNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryException = new Mock<IAdminUserRepository>();
        userRepositoryException.Setup(s => s.GetOne(It.IsAny<User>())).ThrowsAsync(new Exception() {});

        var userRepositoryFailure = new Mock<IAdminUserRepository>();
        userRepositoryFailure.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        userRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(false);

        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        userRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(true);

        // Not Found Test
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), "NOTFOUND");
        Assert.Equal("U001", actual);

        // Delete Exception
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryException.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST"));

        // Delete Failure
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryFailure.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.Equal("U005", actual);

        // Delete Success
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.Empty(actual);
    }

    [Fact]
    public async Task ReactivateTests()
    {
        // Invalid Input Tests
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("U000", actual);
        actual = await service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("U000", actual);

        // Setup
        var userRepositoryNull = new Mock<IAdminUserRepository>();
        userRepositoryNull.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryException = new Mock<IAdminUserRepository>();
        userRepositoryException.Setup(s => s.GetOne(It.IsAny<User>())).ThrowsAsync(new Exception() {});

        var userRepositoryFailure = new Mock<IAdminUserRepository>();
        userRepositoryFailure.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        userRepositoryFailure.Setup(s => s.Reactivate(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(false);

        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        userRepositorySuccess.Setup(s => s.Reactivate(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(true);

        // Not Found Test
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryNull.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), "NOTFOUND");
        Assert.Equal("U001", actual);

        // Reactivate Exception
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryException.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST"));

        // Reactivate Failure
        service = new UsersService(_logger.Object, _configuration.Object, userRepositoryFailure.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.Equal("U006", actual);

        // Reactivate Success
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Reactivate(It.IsAny<string>(), It.IsAny<string>(), "EMAIL@UNIT.TST");
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AuthenticateTests()
    {
        // Failure Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.Authenticate(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Success Test
        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.Authenticate(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
    }

    [Fact]
    public async Task AuthenticateOtpTests()
    {
        // Failure Test
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.AuthenticateOtp(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // Success Test
        var userRepositorySuccess = new Mock<IAdminUserRepository>();
        userRepositorySuccess.Setup(s => s.AuthenticateOtp(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD", Active = true
        });
        service = new UsersService(_logger.Object, _configuration.Object, userRepositorySuccess.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.AuthenticateOtp(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetUserRoleNullOrEmptyTests(string username)
    {
        var service = new UsersService(_logger.Object, _configuration.Object, _userRepository.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetUserRole(It.IsAny<string>(), username);
        Assert.Empty(actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OVERRIDE")]
    public async void GetUserRoleTests(string overrideEmailAddress)
    {
        // Setup
        var settings = new Dictionary<string, string>
        {
            { "OverrideEmailAddress", overrideEmailAddress }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var userRepositoryNotFound = new Mock<IAdminUserRepository>();
        userRepositoryNotFound.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync((User)null);

        var userRepositoryNoRole = new Mock<IAdminUserRepository>();
        userRepositoryNoRole.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User());

        var userRepositoryRole = new Mock<IAdminUserRepository>();
        userRepositoryRole.Setup(s => s.GetOne(It.IsAny<User>())).ReturnsAsync(new User() { RoleCd = "ROLECD" });

        // User Not Found Test
        var service = new UsersService(_logger.Object, configuration, userRepositoryNotFound.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        var actual = await service.GetUserRole(It.IsAny<string>(), "USERNAME");
        Assert.Empty(actual);

        // User Has No Role Test
        service = new UsersService(_logger.Object, configuration, userRepositoryNoRole.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetUserRole(It.IsAny<string>(), "USERNAME");
        Assert.Empty(actual);

        // User Has Role Test
        service = new UsersService(_logger.Object, configuration, userRepositoryRole.Object, _metadataService.Object, _emailService.Object, _keyService.Object);
        actual = await service.GetUserRole(It.IsAny<string>(), "USERNAME");
        Assert.Equal("ROLECD", actual);
    }
}