using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WHLSite.Common.Models;
using WHLSite.Common.Settings;

namespace WHLSite.Services;

public interface IRecaptchaService
{
    Task<RecaptchaVerification> Validate(string requestId, string correlationId, string token, string action);
    Task<RecaptchaVerification> ValidateV2(string requestId, string correlationId, string token, string action);
    RecaptchaVerification ValidateV3(string requestId, string correlationId, string token, string action);
}

[ExcludeFromCodeCoverage]
public class RecaptchaService : IRecaptchaService
{
    private readonly ILogger<RecaptchaService> _logger;
    private readonly HttpClient _client;
    private readonly RecaptchaSettings _recaptchaSettings;

    public RecaptchaService(ILogger<RecaptchaService> logger, HttpClient client, IOptions<RecaptchaSettings> recaptchaSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _recaptchaSettings = recaptchaSettings.Value ?? throw new ArgumentNullException(nameof(recaptchaSettings));
    }

    public async Task<RecaptchaVerification> Validate(string requestId, string correlationId, string token, string action)
    {
        if (!_recaptchaSettings.Enabled)
        {
            _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA is not enabled - check reCAPTCHA settings");
            return new RecaptchaVerification() { Success = true };
        }

        if (string.IsNullOrEmpty((_recaptchaSettings.ProjectId ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA project ID is not specified - check reCAPTCHA settings");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R801"] };
        }

        if (string.IsNullOrEmpty((_recaptchaSettings.Key ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA key is not specified - check reCAPTCHA settings");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R802"] };
        }

        if (string.IsNullOrEmpty((_recaptchaSettings.Secret ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA secret is not specified - check reCAPTCHA settings");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R803"] };
        }

        if (string.IsNullOrEmpty((_recaptchaSettings.VerificationUrl ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA verification URL is not specified - check reCAPTCHA settings");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R804"] };
        }

        if (string.IsNullOrEmpty((token ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA token is not specified - check input");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R805"] };
        }

        if (string.IsNullOrEmpty((action ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA action is not specified - check input");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R806"] };
        }

        try
        {
            if ((_recaptchaSettings.Version ?? "").Trim().Equals("V3", StringComparison.CurrentCultureIgnoreCase))
            {
                // Use V3 Validation
                return ValidateV3(requestId, correlationId, token, action);
            }

            // Default V2 validation
            return await ValidateV2(requestId, correlationId, token, action);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to validate reCAPTCHA!");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R813"] };
        }
    }

    public async Task<RecaptchaVerification> ValidateV2(string requestId, string correlationId, string token, string action)
    {
        _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA V2 is enabled - validating for project: {_recaptchaSettings.ProjectId} using key {_recaptchaSettings.Key.Substring(0, 3)}xxx{_recaptchaSettings.Key.Substring(_recaptchaSettings.Key.Length - 4)}");

        try
        {
            var response = await _client.PostAsync($"?secret={_recaptchaSettings.Secret}&response={token}", new StringContent(""));

            var rawResponseText = await response.Content.ReadAsStringAsync();
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Raw reCAPTCHA response: {rawResponseText}");

            var responseStream = await response.Content.ReadAsStreamAsync();
            var recaptchaVerification = await JsonSerializer.DeserializeAsync<RecaptchaVerification>(responseStream);

            if (recaptchaVerification == null || !recaptchaVerification.Success)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validation was not successful!");
                return recaptchaVerification ?? new RecaptchaVerification() { Success = false, ErrorCodes = ["R813"], RawResponseText = rawResponseText };
            }

            _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validated successfully!");
            recaptchaVerification.RawResponseText = rawResponseText;
            return recaptchaVerification;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to validate reCAPTCHA V2 due to exception!");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R813"], RawResponseText = exception.Message };
        }
    }

    [ExcludeFromCodeCoverage]
    public RecaptchaVerification ValidateV3(string requestId, string correlationId, string token, string action)
    {
        _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA V3 is enabled - validating for project: {_recaptchaSettings.ProjectId} using key {_recaptchaSettings.Key.Substring(0, 3)}xxx{_recaptchaSettings.Key.Substring(_recaptchaSettings.Key.Length - 4)}");

        try
        {
            var client = RecaptchaEnterpriseServiceClient.Create();
            var projectName = new ProjectName(_recaptchaSettings.ProjectId);

            var createAssessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    Event = new Event()
                    {
                        SiteKey = _recaptchaSettings.Key,
                        Token = token,
                        ExpectedAction = action
                    },
                },
                ParentAsProjectName = projectName
            };

            Assessment response = client.CreateAssessment(createAssessmentRequest);
            var recaptchaVerification = new RecaptchaVerification()
            {
                Success = response.TokenProperties.Valid
            };

            if (response.TokenProperties.Valid == false)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: The CreateAssessment call failed because the token was: {response.TokenProperties.InvalidReason}");
                return new RecaptchaVerification() { Success = false, ErrorCodes = ["R811"], RawResponseText = response.TokenProperties.InvalidReason.ToString() };
            }

            // Check if the expected action was executed.
            if (response.TokenProperties.Action != recaptchaVerification.Action)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: The action attribute in reCAPTCHA tag is {response.TokenProperties.Action}, the action attribute in the reCAPTCHA tag does not match the action {recaptchaVerification.Action} you are expecting to score");
                return new RecaptchaVerification() { Success = false, ErrorCodes = ["R812"], RawResponseText = $"Action {response.TokenProperties.Action} does not match request" };
            }

            // Get the risk score and the reasons.
            // For more information on interpreting the assessment,
            // see: https://cloud.google.com/recaptcha/docs/interpret-assessment
            _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: The reCAPTCHA score is: ${response.RiskAnalysis.Score}");

            foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
            {
                _logger.LogDebug(reason.ToString());
            }

            recaptchaVerification.Action = response.TokenProperties.Action;
            recaptchaVerification.ChallengeTimestamp = response.TokenProperties.CreateTime.ToDateTime();
            recaptchaVerification.HostName = response.TokenProperties.Hostname;
            recaptchaVerification.Score = response.RiskAnalysis.Score;
            if (response.RiskAnalysis.Score < 0.3)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: The reCAPTCHA score is too low to allow the action!");
                return new RecaptchaVerification() { Success = false, ErrorCodes = ["R813"], RawResponseText = $"Score {response.RiskAnalysis.Score} is too low" };
            }

            _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validated successfully!");
            return recaptchaVerification;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to validate reCAPTCHA V3 due to exception!");
            return new RecaptchaVerification() { Success = false, ErrorCodes = ["R813"], RawResponseText = exception.Message };
        }
    }
}

public class RecaptchaServiceNoImpl : IRecaptchaService
{
    private readonly ILogger<RecaptchaServiceNoImpl> _logger;

    public RecaptchaServiceNoImpl(ILogger<RecaptchaServiceNoImpl> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<RecaptchaVerification> Validate(string requestId, string correlationId, string token, string action)
    {
        _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validation not implemented!");
        return Task.FromResult(new RecaptchaVerification() { Success = true });
    }

    public Task<RecaptchaVerification> ValidateV2(string requestId, string correlationId, string token, string action)
    {
        _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validated not implemented!");
        return Task.FromResult(new RecaptchaVerification() { Success = true });
    }

    public RecaptchaVerification ValidateV3(string requestId, string correlationId, string token, string action)
    {
        _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: reCAPTCHA validated not implemented!");
        return new RecaptchaVerification() { Success = true };
    }
}