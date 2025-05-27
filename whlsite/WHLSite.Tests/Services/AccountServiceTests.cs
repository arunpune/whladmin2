using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Common.Settings;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<ILogger<AccountService>> _logger = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IKeyService> _keyService = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IPhoneService> _phoneService = new();
    private readonly Mock<IOptions<RecaptchaSettings>> _recaptchaSettingsOptions;
    private readonly RecaptchaSettings _recaptchaSettings;
    private readonly Mock<IRecaptchaService> _recaptchaService = new();

    public AccountServiceTests()
    {
        _recaptchaSettings = new RecaptchaSettings();

        _recaptchaSettingsOptions = new Mock<IOptions<RecaptchaSettings>>();
        _recaptchaSettingsOptions.Setup(s => s.Value).Returns(_recaptchaSettings);
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AccountService(null, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, null, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, null, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, null));

        // Not Null
        var actual = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void AssignMetadataForRegistrationViewModelTests()
    {
        var data = new Dictionary<string, string>
        {
            { "CODE", "DESC" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(data);
        metadataService.Setup(s => s.GetLeadTypes(It.IsAny<bool>())).ReturnsAsync(data);

        var service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);

        var model = new RegistrationViewModel();
        await service.AssignMetadata(model);
        Assert.NotEmpty(model.PhoneNumberTypes);
        Assert.NotEmpty(model.LeadTypes);
    }

    [Theory]
    [InlineData(false, "ACTION", "")]
    [InlineData(true, "ACTION", "1")]
    public void AssignRecaptchaTests(bool enabled, string action, string expected)
    {
        var recaptchaSettingsOptions = new Mock<IOptions<RecaptchaSettings>>();
        recaptchaSettingsOptions.Setup(s => s.Value).Returns(new RecaptchaSettings() { Enabled = enabled, Key = "KEY", TokenUrl = "URL", Version = "VERSION" });

        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, recaptchaSettingsOptions.Object, _recaptchaService.Object);

        var model = new RecaptchaViewModel();
        service.AssignRecaptcha(model, action);
        Assert.Equal("ACTION", model.RecaptchaAction);
        Assert.Equal(expected, model.RecaptchaEnabled);
        Assert.Equal("KEY", model.RecaptchaKey);
        Assert.Empty(model.RecaptchaToken);
        Assert.Equal("URL", model.RecaptchaTokenUrl);
        Assert.Equal("VERSION", model.RecaptchaVersion);
    }

    [Fact]
    public async void GetForRegistrationTests()
    {
        var data = new Dictionary<string, string>
        {
            { "CODE", "DESC" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(data);
        metadataService.Setup(s => s.GetLeadTypes(It.IsAny<bool>())).ReturnsAsync(data);

        var service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.GetForRegistration(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Username);
        Assert.Empty(actual.Password);
        Assert.Empty(actual.ConfirmationPassword);
        Assert.Empty(actual.PhoneNumber);
        Assert.Empty(actual.PhoneNumberExtn);
        Assert.Empty(actual.PhoneNumberTypeCd);
        Assert.Empty(actual.LeadTypeCd);
        Assert.Empty(actual.LeadTypeOther);
        Assert.False(actual.ConsentToReceiveEmailNotifications);
        Assert.False(actual.AcceptTermsAndConditions);
        Assert.NotEmpty(actual.PhoneNumberTypes);
        Assert.NotEmpty(actual.LeadTypes);
        Assert.Equal("REGISTER", actual.RecaptchaAction);
    }

    [Fact]
    public async void RegistrationTest()
    {
        // Null Input
        RegistrationViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // R1021
        model = new RegistrationViewModel() { Username = "USERNAME" };
        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.IsValidUsername(It.IsAny<string>())).Returns(false);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R1021", actual);

        // R0021
        keyService.Setup(s => s.IsValidUsername(It.IsAny<string>())).Returns(true);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R0021", actual);

        // R102
        model = new RegistrationViewModel() { Username = "USERNAME", EmailAddress = "USER@UNIT.TEST" };
        var emailService = new Mock<IEmailService>();
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R102", actual);

        // // R002
        emailService.Setup(s => s.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        // userRepository = new Mock<IUserRepository>();
        // userRepository.Setup(s => s.GetOneByEmailAddress(It.IsAny<string>())).ReturnsAsync(new UserAccount());
        // service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        // actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        // Assert.Equal("R002", actual);

        // R103
        keyService.Setup(s => s.IsValidPassword(It.IsAny<string>())).Returns(false);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R103", actual);

        // R104
        model.Username = "USERNAME";
        model.EmailAddress = "USER@UNIT.TEST";
        model.Password = "PASSWORD";
        model.ConfirmationPassword = "OTHERPASSWORD";
        keyService.Setup(s => s.IsValidPassword("PASSWORD")).Returns(true);
        keyService.Setup(s => s.IsValidPassword("OTHERPASSWORD")).Returns(false);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R104", actual);

        // R105
        keyService.Setup(s => s.IsValidPassword("OTHERPASSWORD")).Returns(true);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R105", actual);

        // R106
        model.ConfirmationPassword = "PASSWORD";
        var phoneService = new Mock<IPhoneService>();
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(false);
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R106", actual);

        var data = new Dictionary<string, string>
        {
            { "CELL", "Cell Phone" },
            { "WORDOFMOUTH", "Word of Mouth" },
            { "WEBSITE", "Website" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ReturnsAsync(data);
        metadataService.Setup(s => s.GetLeadTypes(It.IsAny<bool>())).ReturnsAsync(data);

        // R107
        phoneService.Setup(s => s.IsValidPhoneNumber(It.IsAny<string>())).Returns(true);
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R107", actual);

        // R108
        model.PhoneNumberTypeCd = "CELL";
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R108", actual);

        // R109
        model.LeadTypeCd = "WEBSITE";
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R109", actual);

        // R110
        model.LeadTypeCd = "WORDOFMOUTH";
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R110", actual);

        // R111
        model.ConsentToReceiveEmailNotifications = true;
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R111", actual);

        // reCAPTCHA failure
        model.AcceptTermsAndConditions = true;
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, _userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Add failure
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        userRepository.Setup(s => s.GetOneByEmailAddress(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        userRepository.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        service = new AccountService(_logger.Object, userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R003", actual);

        // Add success
        userRepository.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, metadataService.Object, keyService.Object, emailService.Object, phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public void GetForResendActivationLinkTests()
    {
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = service.GetForResendActivationLink(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Username);
        Assert.Equal("RESENDACTIVATION", actual.RecaptchaAction);
    }

    [Fact]
    public async void ResendActivationLinkTests()
    {
        // Null Input
        ResendActivationViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // Null username (silent)
        model = new ResendActivationViewModel();
        service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Account not found (silent)
        model.Username = "USER@UNIT.TEST";
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // Account activated (silent)
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);

        // reCAPTCHA failure
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Reinitiate failure
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        userRepository.Setup(s => s.ReinitiateActivation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R006", actual);

        // Reinitiate success
        userRepository.Setup(s => s.ReinitiateActivation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void ActivateTests()
    {
        // Account not found by key
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R001", actual);

        // Account verified
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { UsernameVerifiedInd = true });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R001", actual);

        // Key is null
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { ActivationKeyExpiry = null });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R121", actual);

        // Key expired
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { ActivationKeyExpiry = DateTime.MinValue });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R121", actual);

        // Activate failure
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { ActivationKeyExpiry = DateTime.Now.AddMinutes(2) });
        userRepository.Setup(s => s.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R007", actual);

        // Activate success
        userRepository.Setup(s => s.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("", actual);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("R", "R")]
    public void GetForLoginTests(string redirectUrl, string expectedReturnUrl)
    {
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = service.GetForLogin(It.IsAny<string>(), It.IsAny<string>(), redirectUrl);
        Assert.NotNull(actual);
        Assert.Empty(actual.Username);
        Assert.Empty(actual.Password);
        Assert.Equal(expectedReturnUrl, actual.ReturnUrl);
        Assert.Equal("LOGIN", actual.RecaptchaAction);
    }

    [Fact]
    public async void LoginTests()
    {
        // Null Input
        LogInViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // Account not found
        model = new LogInViewModel() { Username = "USERNAME" };
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R001", actual);

        // Account not activated
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = false });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R131", actual);

        // Password mismatch
        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.GetPasswordHash(It.IsAny<string>())).Returns("NEWHASH");
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R132", actual);

        // reCAPCHA failure
        keyService.Setup(s => s.GetPasswordHash(It.IsAny<string>())).Returns("HASH");
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Authenticate failure
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        userRepository.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync((UserAccount)null);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R008", actual);

        // Authenticate success
        userRepository.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(new UserAccount());
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.Login(It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetForChangePasswordTests()
    {
        // Null Test
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.GetForChangePassword(It.IsAny<string>(), It.IsAny<string>(), "USERNAME");
        Assert.Null(actual);

        // Success Test
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { Username = "USERNAME" });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.GetForChangePassword(It.IsAny<string>(), It.IsAny<string>(), "USERNAME");
        Assert.NotNull(actual);
        Assert.Equal("USERNAME", actual.Username);
        Assert.Empty(actual.CurrentPassword);
        Assert.Empty(actual.NewPassword);
        Assert.Empty(actual.ConfirmationPassword);
        Assert.Equal("CHANGEPASSWORD", actual.RecaptchaAction);
    }

    [Fact]
    public async void ChangePasswordTests()
    {
        // Null Input
        ChangePasswordViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // Account not found
        model = new ChangePasswordViewModel() { Username = "USERNAME" };
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R001", actual);

        // Account not activated
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = false });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R131", actual);

        // Invalid current password
        model.CurrentPassword = "PASSWORD";
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.IsValidPassword(It.IsAny<string>())).Returns(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R141", actual);

        // Current password mismatch
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        keyService.Setup(s => s.IsValidPassword("PASSWORD")).Returns(true);
        keyService.Setup(s => s.GetPasswordHash(It.IsAny<string>())).Returns("NEWHASH");
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R142", actual);

        // Invalid new password
        model.CurrentPassword = "PASSWORD";
        model.NewPassword = "NEWPASSWORD";
        keyService.Setup(s => s.GetPasswordHash("PASSWORD")).Returns("HASH");
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R143", actual);

        // New password same as old password
        model.NewPassword = "PASSWORD";
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R144", actual);

        // Invalid confirmation password
        model.NewPassword = "NEWPASSWORD";
        model.ConfirmationPassword = "CONFPASSWORD";
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(true);
        keyService.Setup(s => s.IsValidPassword("CONFPASSWORD")).Returns(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R145", actual);

        // New and confirmation password mismatch
        model.CurrentPassword = "PASSWORD";
        model.NewPassword = "NEWPASSWORD";
        model.ConfirmationPassword = "CONFPASSWORD";
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(true);
        keyService.Setup(s => s.IsValidPassword("CONFPASSWORD")).Returns(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R146", actual);

        // reCAPCHA failure
        model.ConfirmationPassword = "NEWPASSWORD";
        keyService.Setup(s => s.GetPasswordHash("PASSWORD")).Returns("HASH");
        keyService.Setup(s => s.GetPasswordHash("NEWPASSWORD")).Returns("NEWHASH");
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Change password failure
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        userRepository.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R009", actual);

        // Change password success
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount() { UsernameVerifiedInd = true, PasswordHash = "HASH" });
        userRepository.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public void GetForPasswordResetLinkTests()
    {
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = service.GetForPasswordResetLink(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Username);
        Assert.Equal("RESETPASSWORDREQUEST", actual.RecaptchaAction);
    }

    [Fact]
    public async void RequestPasswordResetTests()
    {
        // Null Input
        ResendActivationViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // Account not found
        model = new ResendActivationViewModel() { Username = "USERNAME" };
        var userRepository = new Mock<IUserRepository>();
        // userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        // service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        // actual = await service.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        // Assert.Equal("R001", actual);

        // reCAPCHA failure
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync(new UserAccount());
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Password Reset Request failure
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        userRepository.Setup(s => s.InitiatePasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R010", actual);

        // Password Reset Request success
        userRepository.Setup(s => s.InitiatePasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }

    [Fact]
    public async void GetForPasswordResetRequestTests()
    {
        // Account not found by key
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        var service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R001", actual.Code);

        // Key is null
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKey = null });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R151", actual.Code);

        // Key expired
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.MinValue });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R151", actual.Code);

        // Request failure
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2) });
        userRepository.Setup(s => s.ExchangeKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("R152", actual.Code);

        // Request success
        userRepository.Setup(s => s.ExchangeKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual.Code);
    }

    [Fact]
    public async void ResetPasswordTests()
    {
        // Null Input
        ChangePasswordViewModel model = null;
        var service = new AccountService(_logger.Object, _userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        var actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R101", actual);

        // Account not found
        model = new ChangePasswordViewModel() { Username = "USERNAME" };
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(s => s.GetOne(It.IsAny<string>())).ReturnsAsync((UserAccount)null);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R001", actual);

        // Key is null
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKey = null });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R151", actual);

        // Key expired
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.MinValue });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R151", actual);

        // Invalid new password
        model.NewPassword = "NEWPASSWORD";
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2) });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, _keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R143", actual);

        // New password same as old password
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2), PasswordHash = "HASH" });
        var keyService = new Mock<IKeyService>();
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(true);
        keyService.Setup(s => s.GetPasswordHash("NEWPASSWORD")).Returns("HASH");
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R153", actual);

        // Invalid confirmation password
        model.NewPassword = "NEWPASSWORD";
        model.ConfirmationPassword = "CONFPASSWORD";
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(true);
        keyService.Setup(s => s.IsValidPassword("CONFPASSWORD")).Returns(false);
        keyService.Setup(s => s.GetPasswordHash("NEWPASSWORD")).Returns("NEWHASH");
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R145", actual);

        // New and confirmation password mismatch
        model.CurrentPassword = "PASSWORD";
        model.NewPassword = "NEWPASSWORD";
        model.ConfirmationPassword = "CONFPASSWORD";
        keyService.Setup(s => s.IsValidPassword("NEWPASSWORD")).Returns(true);
        keyService.Setup(s => s.IsValidPassword("CONFPASSWORD")).Returns(true);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, _recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R146", actual);

        // reCAPCHA failure
        model.ConfirmationPassword = "NEWPASSWORD";
        keyService.Setup(s => s.GetPasswordHash("PASSWORD")).Returns("HASH");
        keyService.Setup(s => s.GetPasswordHash("NEWPASSWORD")).Returns("NEWHASH");
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2), PasswordHash = "HASH" });
        var recaptchaService = new Mock<IRecaptchaService>();
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = false, ErrorCodes = [ "CODE" ] });
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("CODE", actual);

        // Change password failure
        recaptchaService.Setup(s => s.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RecaptchaVerification() { Success = true });
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2), PasswordHash = "HASH" });
        userRepository.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(false);
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("R011", actual);

        // Change password success
        userRepository.Setup(s => s.GetOneByKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserAccount { PasswordResetKeyExpiry = DateTime.Now.AddMinutes(2), PasswordHash = "HASH" });
        userRepository.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccount>())).ReturnsAsync(true);
        keyService.Setup(s => s.GetPasswordHash("NEWPASSWORD")).Returns("NEWHASH");
        service = new AccountService(_logger.Object, userRepository.Object, _metadataService.Object, keyService.Object, _emailService.Object, _phoneService.Object, _recaptchaSettingsOptions.Object, recaptchaService.Object);
        actual = await service.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), model);
        Assert.Equal("", actual);
    }
}