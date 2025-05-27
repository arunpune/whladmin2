using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.Common.Services;

public interface IMessageService
{
    string GetMessage(string code);
}

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly Dictionary<string, string> _messages;

    public MessageService(ILogger<MessageService> logger, IOptions<MessageSettings> messageSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (messageSettings != null)
        {
            var settings = messageSettings.Value ?? throw new ArgumentNullException(nameof(messageSettings));
            _messages = settings.Messages;
        }
        else
        {
            throw new ArgumentNullException(nameof(messageSettings));
        }
    }

    public string GetMessage(string code)
    {
        if (_messages != null && _messages.ContainsKey(code))
        {
            return _messages[code];
        }
        return code;
    }
}