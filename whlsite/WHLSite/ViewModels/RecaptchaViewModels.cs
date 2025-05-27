using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class RecaptchaViewModel : ErrorViewModel
{
    public string RecaptchaEnabled { get; set; }
    public string RecaptchaVersion { get; set; }
    public string RecaptchaKey { get; set; }
    public string RecaptchaToken { get; set; }
    public string RecaptchaTokenUrl { get; set; }
    public string RecaptchaAction { get; set; }
}

[ExcludeFromCodeCoverage]
public class RecaptchaTestViewModel : RecaptchaViewModel
{
    public string A { get; set; }
    public DateTime? C { get; set; }
    public string H { get; set; }
    public string[] E { get; set; }
    public string R { get; set; }
    public string Rr { get; set; }
    public bool S { get; set; }
    public float Sc { get; set; }
}