using CBA.Models.AuthModel;

namespace CBA.Models.TokenModel;
public class TokenRequest : AuthResult
{
    
    public new required string Token { get; set; }
    
    public new required string RefreshToken { get; set; }
}