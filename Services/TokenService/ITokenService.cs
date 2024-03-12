using CBA.Models;
using CBA.Models.AuthModel;
using CBA.Models.TokenModel;


namespace CBA.Services;
public interface ITokenService
{
    Task<AuthResult> GenerateTokens(ApplicationUser user);
    Task<AuthResult> VerifyToken(TokenRequest tokenRequest);
    Task<LogoutResponse>  RevokeToken(string userName);
}