
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WHLAdmin.Common.Filters;
using WHLAdmin.Common.Settings;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers
{
    [ExcludeFromCodeCoverage]
    public class EmfluenceTestController : Controller
    {
        private readonly IMasterConfigService _masterConfigService;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string EmfluenceApiClient = "EMFLUENCEAPI";
        private const string HealthCheckApiEndpoint = "/helper/ping";
        private const string ContactLookupApiEndpoint = "/contacts/lookup";
        private const string ContactSearchApiEndpoint = "/contacts/search";
        private const string GroupLookupApiEndpoint = "/groups/lookup";
        private const string GroupSearchApiEndpoint = "/groups/search";

        public EmfluenceTestController(IMasterConfigService masterConfigService, IHttpClientFactory httpClientFactory)
        {
            _masterConfigService = masterConfigService ?? throw new ArgumentNullException(nameof(masterConfigService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        [HttpGet]
        [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
        public async Task<IActionResult> Index()
        {
            var model = new EmfluenceViewModel();
            try
            {
                model.Settings = await _masterConfigService.GetEmfluenceSettings();

                try
                {
                    using var httpClient = _httpClientFactory.CreateClient(EmfluenceApiClient);
                    httpClient.BaseAddress = new Uri(model.Settings.ApiUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "1024E3DB-374C-4DEB-916B-BDC9F5EC68AA");
                    var response = await httpClient.GetAsync(HealthCheckApiEndpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        model.Success = 1;
                        model.Status = "Available";
                    }
                    else
                    {
                        model.Success = -1;
                        model.Status = "Unavailable";
                    }
                }
                catch (Exception apiException)
                {
                    model.Success = -1;
                    model.Code = "EX000";
                    model.Message = "Exception when retrieving Emfluence API status";
                    model.Details = apiException.ToString();
                }
                return View(model);
            }
            catch (Exception exception)
            {
                return View("Error", new ErrorViewModel() { Code = "EX000", Message = "Exception when loading page", Details = exception.ToString() });
            }
        }

        [HttpGet]
        [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
        public async Task<IActionResult> GetStatus()
        {
            var model = new EmfluenceViewModel();
            try
            {
                model.Settings = await _masterConfigService.GetEmfluenceSettings();

                using var httpClient = GetHttpClient(model.Settings);
                var response = await httpClient.GetAsync(HealthCheckApiEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    model.Success = 1;
                    model.Status = "Available";
                    return Ok(model);
                }

                model.Success = -1;
                model.Status = "Unavailable";
                return StatusCode(500, model);
            }
            catch (Exception apiException)
            {
                model.Code = "EX000";
                model.Message = "Exception when retrieving Emfluence API status";
                model.Details = apiException.ToString();
                return StatusCode(500, model);
            }
        }

        [HttpPost]
        [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
        public async Task<IActionResult> LookupContact([FromForm] EmfluenceContactLookupViewModel model)
        {
            try
            {
                model.Settings = await _masterConfigService.GetEmfluenceSettings();

                model.EmailAddress = (model.EmailAddress ?? "").Trim();
                model.ContactId = model.ContactId < 0 ? 0 : model.ContactId;

                var json = JsonSerializer.Serialize(new { email = model.EmailAddress });
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                using var httpClient = GetHttpClient(model.Settings);
                var response = model.ContactId > 0
                    ? await httpClient.GetAsync(model.Settings.ApiUrl + ContactLookupApiEndpoint + $"?contactID={model.ContactId}")
                        : await httpClient.PostAsync(model.Settings.ApiUrl + ContactLookupApiEndpoint, requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var contact = JsonSerializer.Deserialize<EmfluenceResponseViewModel<EmfluenceContactViewModel>>(responseString);
                    return Ok(contact);
                }

                model.Success = -1;
                model.Status = "Not Found";
                return StatusCode(500, model);
            }
            catch (Exception apiException)
            {
                model.Code = "EX000";
                model.Message = "Exception when looking up contact in Emfluence";
                model.Details = apiException.ToString();
                return StatusCode(500, model);
            }
        }

        [HttpPost]
        [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
        public async Task<IActionResult> LookupGroup([FromForm] EmfluenceGroupLookupViewModel model)
        {
            try
            {
                model.Settings = await _masterConfigService.GetEmfluenceSettings();

                model.GroupName = (model.GroupName ?? "").Trim();
                model.GroupId = model.GroupId < 0 ? 0 : model.GroupId;

                var json = JsonSerializer.Serialize(new { groupName = model.GroupName });
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                using var httpClient = GetHttpClient(model.Settings);
                if (model.GroupId > 0)
                {
                    var response = await httpClient.GetAsync(model.Settings.ApiUrl + GroupLookupApiEndpoint + $"?groupID={model.GroupId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var group = JsonSerializer.Deserialize<EmfluenceResponseViewModel<EmfluenceGroupViewModel>>(responseString);
                        return Ok(group);
                    }
                }
                else
                {
                    var response = await httpClient.GetAsync(model.Settings.ApiUrl + GroupSearchApiEndpoint + (model.GroupName.Length > 0 ? $"?groupName={model.GroupName}" : ""));
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var group = JsonSerializer.Deserialize<EmfluenceListResponseViewModel<EmfluenceGroupViewModel>>(responseString);
                        return Ok(group);
                    }
                }

                model.Success = -1;
                model.Status = "Not Found";
                return StatusCode(500, model);
            }
            catch (Exception apiException)
            {
                model.Code = "EX000";
                model.Message = "Exception when looking up group in Emfluence";
                model.Details = apiException.ToString();
                return StatusCode(500, model);
            }
        }

        private HttpClient GetHttpClient(EmfluenceSettings settings)
        {
            var httpClient = _httpClientFactory.CreateClient(EmfluenceApiClient);
            httpClient.BaseAddress = new Uri(settings.ApiUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
            return httpClient;
        }
    }
}