using System.Security.Cryptography;
using System.Text;

namespace CBA.Services;

public interface IPasswordService{
    string HashPassword(string password);
    bool IsValidPassword(string password, string hashPass);

}