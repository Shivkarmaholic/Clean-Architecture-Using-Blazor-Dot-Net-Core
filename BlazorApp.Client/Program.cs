using BlazorApp.Client.Components;
using BlazorApp.Client.Endpoints;
using BlazorApp.Client.Middleware;
using BlazorApp.Client.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Razor Components (.NET 8 Blazor)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Localization: point to Resources folder (so SharedResource.<culture>.resx is discovered)
builder.Services.AddLocalization();

var supportedCultures = new[] { "en-US", "fr-FR", "hi-IN" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.SetDefaultCulture("en-US");
    options.ApplyCurrentCultureToResponseHeaders = true;

    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

// Theme + HttpContext access
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ThemeState>();

// HttpClient for API
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "https://localhost:7043";
builder.Services.AddHttpClient<CustomerService>(c => c.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

// Global exception handler for endpoints
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Extracted endpoints
app.MapCultureEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
