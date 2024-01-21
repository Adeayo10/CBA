
namespace CBA.Models.AuthModel;
public class AuthResult
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public DateTime? ExpiryDate { get; set; } 
    public List<string>? Errors { get; set; }
}
