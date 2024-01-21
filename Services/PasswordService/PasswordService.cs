using System.Security.Cryptography;
using System.Text;

namespace CBA.Services;
public class PasswordService: IPasswordService
{
    private ILogger<PasswordService> _logger;
    public PasswordService(ILogger<PasswordService> logger)
    {
        _logger = logger;
        _logger.LogInformation("PasswordService constructor called");
    }
    public string HashPassword(string password)
    {
        //var salt = new byte[20];
        var salt = RandomNumberGenerator.GetBytes(20);
        
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);

        
        byte[] hashBytes = new byte[40];
        Array.Copy(salt, 0, hashBytes, 0, 20);
        Array.Copy(hash, 0, hashBytes, 20, 20);


        string hashPass = Convert.ToBase64String(hashBytes);

        _logger.LogInformation("HashPassword called, Password hashed");
        return hashPass;
    }
    public bool IsValidPassword(string password, string hashPass)
    {
        bool result = true;
     
        byte[] hashBytes = Convert.FromBase64String(hashPass);
       
        byte[] salt = new byte[20];
        Array.Copy(hashBytes, 0, salt, 0, 20);
        
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);
      
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 20] != hash[i])
            {
                _logger.LogInformation("IsValidPassword, Password is not valid");
                return !result;
            }
        }
        _logger.LogInformation("IsValidPassword called, Password is valid");
        return result;
    }
}