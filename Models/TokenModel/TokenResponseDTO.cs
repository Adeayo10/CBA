

namespace CBA.Models.TokenModel
{
    public class TokenResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiryDate { get; internal set; }
    }
}