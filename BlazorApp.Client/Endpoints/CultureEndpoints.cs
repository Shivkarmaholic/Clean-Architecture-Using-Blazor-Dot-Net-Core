using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace BlazorApp.Client.Endpoints;

public static class CultureEndpoints
{
    public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/set-culture", (HttpContext http, string culture, string? redirectUri) =>
        {
            AppendCultureCookie(http, culture);
            return Results.Redirect(string.IsNullOrWhiteSpace(redirectUri) ? "/" : redirectUri);
        });

        endpoints.MapGet("/set-theme", (HttpContext http, string theme, string? redirectUri) =>
        {
            AppendThemeCookie(http, theme);
            return Results.Redirect(string.IsNullOrWhiteSpace(redirectUri) ? "/" : redirectUri);
        });

        return endpoints;
    }

    private static void AppendCultureCookie(HttpContext http, string culture)
    {
        var cookie = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
        http.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookie, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/",
            HttpOnly = false,
            Secure = http.Request.IsHttps,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }

    private static void AppendThemeCookie(HttpContext http, string theme)
    {
        http.Response.Cookies.Append(".Theme", theme, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/",
            HttpOnly = false,
            Secure = http.Request.IsHttps,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }
}