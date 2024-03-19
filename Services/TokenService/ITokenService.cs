using CBA.Models;
using CBA.Models.AuthModel;
using CBA.Models.TokenModel;


namespace CBA.Services;
public interface ITokenService
{
    Task<AuthResult> GenerateTokensAsync(ApplicationUser user);
    Task<AuthResult> VerifyTokenAsync(TokenRequest tokenRequest);
    Task<LogoutResponse>  RevokeTokenAsync(string userName);
}