using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace WHLAdmin.Common.Services;

public interface IPhoneService
{
    bool IsValidPhoneNumber(string requestId, string correlationId, string input);
}

public partial class PhoneService : IPhoneService
{
    private readonly ILogger<PhoneService> _logger;

    public PhoneService(ILogger<PhoneService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsValidPhoneNumber(string requestId, string correlationId, string input)
    {
        input = (input ?? "").Trim();
        return PhoneNumberRegex().IsMatch(input);
    }

    [ExcludeFromCodeCoverage]
    [GeneratedRegex(@"^(\d){10}$")]
    private static partial Regex PhoneNumberRegex();
}