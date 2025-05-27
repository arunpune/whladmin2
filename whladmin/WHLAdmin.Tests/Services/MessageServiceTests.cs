using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WHLAdmin.Common.Services;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<ILogger<MessageService>> _logger = new();
    private readonly Mock<Dictionary<string, string>> _messages;

    public MessageServiceTests()
    {
        _messages = new();
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new MessageService(null, null));
        Assert.Throws<ArgumentNullException>(() => new MessageService(_logger.Object, null));

        // Invalid Options
        var options = Options.Create((MessageSettings)null);
        Assert.Throws<ArgumentNullException>(() => new MessageService(_logger.Object, options));

        // Not Null
        var messageSettings = new MessageSettings();
        options = Options.Create(messageSettings);
        var actual = new MessageService(_logger.Object, options);
        Assert.NotNull(actual);
    }

    [Fact]
    public void GetMessageTests()
    {
        // Not found Test
        var messageSettings = new MessageSettings();
        var options = Options.Create(messageSettings);
        var service = new MessageService(_logger.Object, options);
        var actual = service.GetMessage("NOTFOUND");
        Assert.Equal("NOTFOUND", actual);

        // Found Test
        messageSettings = new MessageSettings()
        {
            Messages = new Dictionary<string, string>()
            {
                { "FOUND", "Found" }
            }
        };
        options = Options.Create(messageSettings);
        service = new MessageService(_logger.Object, options);
        actual = service.GetMessage("FOUND");
        Assert.Equal("Found", actual);
    }
}