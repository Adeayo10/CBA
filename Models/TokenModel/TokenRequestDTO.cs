using CBA.Models.AuthModel;

namespace CBA.Models.TokenModel;
public class TokenRequest
{
    
    public  required string Token { get; set; }
    
    public  required string RefreshToken { get; set; }
}