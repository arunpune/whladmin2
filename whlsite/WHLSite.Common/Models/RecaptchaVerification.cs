using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class RecaptchaVerification
{
    [JsonIgnore]
    public string Token { get; set; }

    [JsonIgnore]
    public string RawResponseText { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("score")]
    public float Score { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTime? ChallengeTimestamp { get; set; }

    [JsonPropertyName("hostname")]
    public string HostName { get; set; }

    [JsonPropertyName("error-codes")]
    public string[] ErrorCodes { get; set; }
}