using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _logger = new();
    private readonly Mock<INotificationConfigRepository> _notificationConfigRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettingsOptions;
    private readonly Mock<SmtpSettings> _smtpSettings;

    public EmailServiceTests()
    {
        _smtpSettings = new();
        _smtpSettingsOptions = new Mock<IOptions<SmtpSettings>>();
        _smtpSettingsOptions.Setup(s => s.Value).Returns(_smtpSettings.Object);
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new EmailService(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new EmailService(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new EmailService(_logger.Object, _notificationConfigRepository.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new EmailService(_logger.Object, _notificationConfigRepository.Object, _userRepository.Object, null));

        // Invalid Options
        var smtpSettingsOptions = Options.Create((SmtpSettings)null);
        Assert.Throws<ArgumentNullException>(() => new EmailService(_logger.Object, _notificationConfigRepository.Object, _userRepository.Object, smtpSettingsOptions));

        // Not Null
        var actual = new EmailService(_logger.Object, _notificationConfigRepository.Object, _userRepository.Object, _smtpSettingsOptions.Object);
        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("a", false)]
    [InlineData("a.b", false)]
    [InlineData("a@b", true)]
    [InlineData("a@b.c", true)]
    public void IsValidEmailAddressTests(string emailAddress, bool expected)
    {
        var service = new EmailService(_logger.Object, _notificationConfigRepository.Object, _userRepository.Object, _smtpSettingsOptions.Object);
        var actual = service.IsValidEmailAddress(It.IsAny<string>(), It.IsAny<string>(), emailAddress);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, -1, null, null, null, null, null, null, false, "E002")]
    [InlineData("", -1, null, null, null, null, null, null, false, "E002")]
    [InlineData(" ", -1, null, null, null, null, null, null, false, "E002")]
    [InlineData("HOST", -1, null, null, null, null, null, null, false, "E003")]
    [InlineData("HOST", 0, null, null, null, null, null, null, false, "E003")]
    [InlineData("HOST", 25, null, null, null, null, null, null, true, "E004")]
    [InlineData("HOST", 25, "", null, null, null, null, null, true, "E004")]
    [InlineData("HOST", 25, " ", null, null, null, null, null, true, "E004")]
    [InlineData("HOST", 25, "UN", null, null, null, null, null, true, "E005")]
    [InlineData("HOST", 25, "UN", "", null, null, null, null, true, "E005")]
    [InlineData("HOST", 25, "UN", " ", null, null, null, null, true, "E005")]
    [InlineData("HOST", 25, "UN", "PW", null, null, null, null, true, "E006")]
    [InlineData("HOST", 25, "UN", "PW", "", null, null, null, true, "E006")]
    [InlineData("HOST", 25, "UN", "PW", " ", null, null, null, true, "E006")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", null, null, null, true, "E007")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "", null, null, true, "E007")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", " ", null, null, true, "E007")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", null, null, true, "E008")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", "", null, true, "E008")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", " ", null, true, "E008")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", "SUBJECT", null, true, "E009")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", "SUBJECT", "", true, "E009")]
    [InlineData("HOST", 25, "UN", "PW", "FROM", "TO", "SUBJECT", " ", true, "E009")]
    public async Task SendEmailTests(string host, int port, string username, string password, string fromAddress, string toAddresses, string subject, string body, bool useAuth, string expectedCode)
    {
        var smtpSettings = new SmtpSettings()
        {
            SmtpHost = host,
            SmtpPort = port,
            SmtpUsername = username,
            SmtpPassword = password,
            SmtpFromAddress = fromAddress,
            UseAuthentication = useAuth
        };
        var mailMessage = new MailMessage()
        {
            ToAddresses = toAddresses,
            Subject = subject,
            Body = body
        };
        var smtpSettingsOptions = new Mock<IOptions<SmtpSettings>>();
        smtpSettingsOptions.Setup(s => s.Value).Returns(smtpSettings);
        var service = new EmailService(_logger.Object, _notificationConfigRepository.Object, _userRepository.Object, smtpSettingsOptions.Object);
        var actual = await service.SendEmail(It.IsAny<string>(), It.IsAny<string>(), mailMessage);
        Assert.Equal(expectedCode, actual);
    }
}