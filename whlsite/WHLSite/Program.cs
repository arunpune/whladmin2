using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using WHLSite.Common.Providers;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Common.Settings;
using WHLSite.Services;

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Add the environment-specific settings file
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true);

    // Add the JSON messages file
    builder.Configuration.AddJsonFile("messages.json",
        optional: true,
        reloadOnChange: true);

    // SeriLog congiguration
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    // Add Settings
    builder.Services.Configure<GoogleTranslateSettings>(builder.Configuration.GetSection("GoogleTranslateSettings"));
    builder.Services.Configure<MessageSettings>(builder.Configuration.GetSection("MessageSettings"));
    builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("RecaptchaSettings"));
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

    var recaptchaEnabled = builder.Configuration.GetSection("RecaptchaSettings")?.GetValue<bool>("Enabled") ?? false;
    if (recaptchaEnabled)
    {
        builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>((serviceProvider, client) =>
        {
            var recaptchaSettings = serviceProvider.GetRequiredService<IOptions<RecaptchaSettings>>().Value;
            client.BaseAddress = new Uri(recaptchaSettings.VerificationUrl);
        });
    }
    else
    {
        builder.Services.AddSingleton<IRecaptchaService, RecaptchaServiceNoImpl>();
    }

    // Add repositories to the container.
    builder.Services.AddSingleton<IDbProvider, SqlDbProvider>();
    builder.Services.AddSingleton<IAmortizationRepository, AmortizationRepository>();
    builder.Services.AddSingleton<IFaqRepository, FaqRepository>();
    builder.Services.AddSingleton<IHouseholdRepository, HouseholdRepository>();
    builder.Services.AddSingleton<IHousingApplicationRepository, HousingApplicationRepository>();
    builder.Services.AddSingleton<IListingAccessibilityRepository, ListingAccessibilityRepository>();
    builder.Services.AddSingleton<IListingAmenityRepository, ListingAmenityRepository>();
    builder.Services.AddSingleton<IListingDeclarationRepository, ListingDeclarationRepository>();
    builder.Services.AddSingleton<IListingDisclosureRepository, ListingDisclosureRepository>();
    builder.Services.AddSingleton<IListingDocumentRepository, ListingDocumentRepository>();
    builder.Services.AddSingleton<IListingDocumentTypeRepository, ListingDocumentTypeRepository>();
    builder.Services.AddSingleton<IListingFundingSourceRepository, ListingFundingSourceRepository>();
    builder.Services.AddSingleton<IListingImageRepository, ListingImageRepository>();
    builder.Services.AddSingleton<IListingRepository, ListingRepository>();
    builder.Services.AddSingleton<IListingUnitHouseholdRepository, ListingUnitHouseholdRepository>();
    builder.Services.AddSingleton<IListingUnitRepository, ListingUnitRepository>();
    builder.Services.AddSingleton<IMasterConfigRepository, MasterConfigRepository>();
    builder.Services.AddSingleton<IMetadataRepository, MetadataRepository>();
    builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
    builder.Services.AddSingleton<IQuoteRepository, QuoteRepository>();
    builder.Services.AddSingleton<IResourceRepository, ResourceRepository>();
    builder.Services.AddSingleton<ISystemRepository, SystemRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddSingleton<IVideoRepository, VideoRepository>();

    // Add services to the container.
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IAmortizationsService, AmortizationsService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IFaqService, FaqService>();
    builder.Services.AddScoped<IHouseholdService, HouseholdService>();
    builder.Services.AddScoped<IHousingApplicationService, HousingApplicationService>();
    builder.Services.AddScoped<IKeyService, KeyService>();
    builder.Services.AddScoped<IListingService, ListingService>();
    builder.Services.AddSingleton<IMasterConfigService, MasterConfigService>();
    builder.Services.AddSingleton<IMessageService, MessageService>();
    builder.Services.AddSingleton<IMetadataService, MetadataService>();
    builder.Services.AddScoped<IPhoneService, PhoneService>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IQuoteService, QuoteService>();
    builder.Services.AddScoped<IResourceService, ResourceService>();
    builder.Services.AddSingleton<IUiHelperService, UiHelperService>();
    builder.Services.AddScoped<IVideoService, VideoService>();
    builder.Services.AddControllersWithViews();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.Cookie.Name = "whlcookie";
                            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                            options.SlidingExpiration = true;
                            options.AccessDeniedPath = "/Forbidden/";
                            options.LoginPath = "/Account/LogIn";
                            options.LogoutPath = "/Account/Logout";
                            options.ReturnUrlParameter = "r";
                        });

    // builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
