using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WHLSite.Common.Settings;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

[AllowAnonymous]
[ExcludeFromCodeCoverage]
public class RecaptchaTestController : Controller
{
    private readonly ILogger<RecaptchaTestController> _logger;
    private readonly RecaptchaSettings _recaptchaSettings;
    private readonly IRecaptchaService _service;

    public RecaptchaTestController(ILogger<RecaptchaTestController> logger, IOptions<RecaptchaSettings> recaptchaSettingsOptions, IRecaptchaService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recaptchaSettings = recaptchaSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(recaptchaSettingsOptions));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new RecaptchaTestViewModel()
        {
            RecaptchaAction = "RECAPTCHATEST",
            RecaptchaEnabled = _recaptchaSettings.Enabled ? "1" : "",
            RecaptchaKey = _recaptchaSettings.Key,
            RecaptchaToken = "",
            RecaptchaTokenUrl = _recaptchaSettings.TokenUrl,
            RecaptchaVersion = _recaptchaSettings.Version
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(RecaptchaTestViewModel model)
    {
        var recaptchaVerification = await _service.Validate(model.RecaptchaAction, model.RecaptchaAction, model.RecaptchaToken, model.RecaptchaAction);
        model.R = recaptchaVerification.ErrorCodes?.AsEnumerable()?.FirstOrDefault() ?? "";
        model.A = recaptchaVerification.Action;
        model.C = recaptchaVerification.ChallengeTimestamp;
        model.H = recaptchaVerification.HostName;
        model.E = recaptchaVerification.ErrorCodes;
        model.Rr = recaptchaVerification.RawResponseText;
        model.S = recaptchaVerification.Success;
        model.Sc = recaptchaVerification.Score;
        model.RecaptchaAction = "RECAPTCHATEST";
        model.RecaptchaEnabled = _recaptchaSettings.Enabled ? "1" : "";
        model.RecaptchaKey = _recaptchaSettings.Key;
        model.RecaptchaToken = "";
        model.RecaptchaTokenUrl = _recaptchaSettings.TokenUrl;
        model.RecaptchaVersion = _recaptchaSettings.Version;
        return View(model);

        // var body = new { secret = model.K, response = model.T };
        // var jsonBody = JsonSerializer.Serialize(body);
        // var content = new StringContent(jsonBody);

        // using var client = new HttpClient();
        // client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
        // var response = await client.PostAsync($"?secret={model.S}&response={model.T}", new StringContent(""));
        // client.Dispose();
        // var t = await response.Content.ReadAsStringAsync();
        // model.R = t;
        // var r = await response.Content.ReadAsStreamAsync();
        // var j = await JsonSerializer.DeserializeAsync<RecaptchaTestViewModel>(r);

        // return View(model);
    }
}