using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace BlazorApp.Client.Services;

public sealed class ThemeState
{
    private const string CookieName = ".Theme";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ThemeState(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        CurrentTheme = ReadThemeFromCookie() ?? Theme.Light;
    }

    public Theme CurrentTheme { get; private set; }

    public event Action? Changed;

    public void SetTheme(Theme theme)
    {
        if (CurrentTheme == theme) return;
        CurrentTheme = theme;
        Changed?.Invoke();
    }

    private Theme? ReadThemeFromCookie()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var themeValue = ctx?.Request.Cookies[CookieName];
        return Enum.TryParse<Theme>(themeValue, ignoreCase: true, out var t) ? t : null;
    }
}