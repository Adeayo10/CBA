namespace CBA.Services;

public interface ICookieService
{
    void SetCookie(string cookieName, string value, int? expireTime);
    string GetCookie(string cookieName);
    void RemoveCookie(string cookieName);
    void ModifyCookie(string cookieName, string value, int? expireTime);
    void RemoveAllCookies();
}