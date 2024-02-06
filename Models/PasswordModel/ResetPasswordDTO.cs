
namespace CBA.Models
{
    public class ResetPasswordDTO
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

       

    }
}