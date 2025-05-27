using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WHLSite.Controllers;

[AllowAnonymous]
[ExcludeFromCodeCoverage]
public class ArcGISTestController : Controller
{
    private readonly ILogger<ArcGISTestController> _logger;

    public ArcGISTestController(ILogger<ArcGISTestController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}