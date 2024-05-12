using Microsoft.AspNetCore.DataProtection;

namespace CBA.Services;

public class CookieService: ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieService> _logger;
    private readonly IDataProtector _protector;
    public CookieService(IHttpContextAccessor httpContextAccessor, ILogger<CookieService> logger, IDataProtectionProvider dataProtectionProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _protector = dataProtectionProvider.CreateProtector("CookieProtector");
    }
    public void SetCookie(string cookieName, string value, int? expireTime)
    {
        _logger.LogInformation("SetCookie method called");
        
        _protector.Protect(value);

        CookieOptions option = new()
        {
            HttpOnly = true,
            Secure = true,
           // SameSite = SameSiteMode.Strict

        };
        if (expireTime.HasValue)
            option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
        else
            option.Expires = DateTime.Now.AddMilliseconds(10);
        
        _httpContextAccessor.HttpContext?.Response?.Cookies.Append(cookieName, value, option);
    }
    public string GetCookie(string cookieName)
    {
        _logger.LogInformation("GetCookie method called");
        string? encryptedValue = _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        if (encryptedValue is null)
        {
            _logger.LogInformation("Cookie not found");
            return new string("Cookie not found");
        }
        var decryptedValue = _protector.Unprotect(encryptedValue!);
        return encryptedValue != null ? decryptedValue : null!;
    }
    public void RemoveCookie(string cookieName)
    {
        _logger.LogInformation("RemoveCookie method called");
        _httpContextAccessor.HttpContext?.Response?.Cookies.Delete(cookieName);
    }   

    public void RemoveAllCookies()
    {
        _logger.LogInformation("RemoveAllCookies method called");
        var cookieCollection = _httpContextAccessor.HttpContext?.Request?.Cookies;
        foreach (var cookie in cookieCollection!)
        {
            _httpContextAccessor.HttpContext?.Response?.Cookies.Delete(cookie.Key);
        }
        
    }
    public void ModifyCookie(string cookieName, string value, int? expireTime)
    {
        _logger.LogInformation("ModifyCookie method called");
        RemoveCookie(cookieName);
        SetCookie(cookieName, value, expireTime);
    }
}