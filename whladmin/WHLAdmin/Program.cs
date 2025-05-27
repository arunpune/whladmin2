using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Okta.AspNetCore;
using Serilog;
using Serilog.Events;
using WHLAdmin.Common.Providers;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Common.Settings;
using WHLAdmin.Services;

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

    // Add the messages file
    builder.Configuration.AddJsonFile("messages.json",
        optional: true,
        reloadOnChange: true);

    // SeriLog congiguration
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    // Http Clients
    builder.Services.AddHttpClient();
    builder.Services.AddHttpClient("EMFLUENCEAPI");

    // Add settings to the container.
    builder.Services.Configure<MessageSettings>(builder.Configuration.GetSection("MessageSettings"));
    builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("RecaptchaSettings"));
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

    // Add repositories to the container.
    builder.Services.AddSingleton<IDbProvider, SqlDbProvider>();
    builder.Services.AddSingleton<IAdminUserRepository, AdminUserRepository>();
    builder.Services.AddSingleton<IAmenityRepository, AmenityRepository>();
    builder.Services.AddSingleton<IAmiConfigRepository, AmiConfigRepository>();
    builder.Services.AddSingleton<IAmortizationRepository, AmortizationRepository>();
    builder.Services.AddSingleton<IAuditRepository, AuditRepository>();
    builder.Services.AddSingleton<IDocumentTypeRepository, DocumentTypeRepository>();
    builder.Services.AddSingleton<IFaqConfigRepository, FaqConfigRepository>();
    builder.Services.AddSingleton<IFundingSourceRepository, FundingSourceRepository>();
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
    builder.Services.AddSingleton<ILotteryRepository, LotteryRepository>();
    builder.Services.AddSingleton<IMarketingAgentRepository, MarketingAgentRepository>();
    builder.Services.AddSingleton<IMasterConfigRepository, MasterConfigRepository>();
    builder.Services.AddSingleton<IMetadataRepository, MetadataRepository>();
    builder.Services.AddSingleton<INoteRepository, NoteRepository>();
    builder.Services.AddSingleton<INotificationConfigRepository, NotificationConfigRepository>();
    builder.Services.AddSingleton<IQuestionConfigRepository, QuestionConfigRepository>();
    builder.Services.AddSingleton<IQuoteConfigRepository, QuoteConfigRepository>();
    builder.Services.AddSingleton<IReportRepository, ReportRepository>();
    builder.Services.AddSingleton<IResourceConfigRepository, ResourceConfigRepository>();
    builder.Services.AddSingleton<ISystemRepository, SystemRepository>();
    builder.Services.AddSingleton<IUserRepository, UserRepository>();
    builder.Services.AddSingleton<IVideoConfigRepository, VideoConfigRepository>();

    // Add services to the container.
    builder.Services.AddSingleton<IAmenitiesService, AmenitiesService>();
    builder.Services.AddSingleton<IAmiConfigsService, AmiConfigsService>();
    builder.Services.AddSingleton<IAmortizationsService, AmortizationsService>();
    builder.Services.AddSingleton<IAuditService, AuditService>();
    builder.Services.AddSingleton<IDocumentTypesService, DocumentTypesService>();
    builder.Services.AddSingleton<IEmailService, EmailService>();
    builder.Services.AddSingleton<IFaqConfigsService, FaqConfigsService>();
    builder.Services.AddSingleton<IFundingSourcesService, FundingSourcesService>();
    builder.Services.AddSingleton<IHousingApplicationsService, HousingApplicationsService>();
    builder.Services.AddSingleton<IKeyService, KeyService>();
    builder.Services.AddSingleton<IListingsService, ListingsService>();
    builder.Services.AddSingleton<ILotteriesService, LotteriesService>();
    builder.Services.AddSingleton<IMarketingAgentsService, MarketingAgentsService>();
    builder.Services.AddSingleton<IMasterConfigService, MasterConfigService>();
    builder.Services.AddSingleton<IMessageService, MessageService>();
    builder.Services.AddSingleton<IMetadataService, MetadataService>();
    builder.Services.AddSingleton<INotesService, NotesService>();
    builder.Services.AddSingleton<INotificationConfigsService, NotificationConfigsService>();
    builder.Services.AddSingleton<IPhoneService, PhoneService>();
    builder.Services.AddSingleton<IQuestionConfigsService, QuestionConfigsService>();
    builder.Services.AddSingleton<IQuoteConfigsService, QuoteConfigsService>();
    builder.Services.AddSingleton<IReportsService, ReportsService>();
    builder.Services.AddSingleton<IResourceConfigsService, ResourceConfigsService>();
    builder.Services.AddSingleton<IUiHelperService, UiHelperService>();
    builder.Services.AddSingleton<IUsersService, UsersService>();
    builder.Services.AddSingleton<IVideoConfigsService, VideoConfigsService>();

    var authMode = builder.Configuration.GetValue<string>("AuthMode") ?? "";
    switch (authMode)
    {
        case "OKTA":
            // Add Okta Authentication scheme
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "whladmincookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Expiration = TimeSpan.FromMinutes(20);
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOktaMvc(new OktaMvcOptions
            {
                // Replace these values with your Okta configuration
                OktaDomain = builder.Configuration.GetValue<string>("Okta:OktaDomain"),
                AuthorizationServerId = builder.Configuration.GetValue<string>("Okta:AuthorizationServerId"),
                ClientId = builder.Configuration.GetValue<string>("Okta:ClientId"),
                ClientSecret = builder.Configuration.GetValue<string>("Okta:ClientSecret"),
                Scope = ["openid", "profile", "email"],
            });
            break;

        case "WINDOWS":
            // Add Windows authentiation scheme
            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });
            break;

        default:
            // Add Basic auth scheme
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "whladmincookie";
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = new PathString("/Home/Forbidden/");
                options.LoginPath = new PathString("/Home/LogIn");
                options.LogoutPath = new PathString("/Home/Logout");
                options.ReturnUrlParameter = "r";
            });
            break;
    }

    builder.Services.AddControllersWithViews();

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddWebOptimizer(minifyJavaScript:false, minifyCss:false);
    }
    else
    {
        builder.Services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddCssBundle("/css/whladmin.css", "css/site.css");
            pipeline.AddJavaScriptBundle("/js/whladmin.js", "js/site.js");
            pipeline.MinifyCssFiles();
            pipeline.MinifyJsFiles();
        });
    }

    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseWebOptimizer();
    app.UseStaticFiles();

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();
    //app.UseSession().UseMiddleware<AdminSessionMiddleware>();

    app.MapControllerRoute(
        name: "authorization-code",
        pattern: "authorization-code/callback",
        defaults: new { controller = "Home", action = "OktaSignInCallback" }
    );
    app.MapControllerRoute(
        name: "authorization-code",
        pattern: "signout/callback",
        defaults: new { controller = "Home", action = "OktaSignOutCallback" }
    );
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );

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