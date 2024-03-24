
namespace CBA.Models.AuthModel
{
    public class  LoginResponse: AuthResult
    {
        public string? UserId {get; set;}

    }
    public class LogoutResponse
    {
        public string Message { get; set; }
        public string StatusCode { get; set;}
        public bool Success { get; set; }
    }
}