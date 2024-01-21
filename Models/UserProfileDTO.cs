using System.ComponentModel.DataAnnotations;
using CBA.Enums;

namespace CBA.Models;

public class UserProfile
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required string FullName  { get; set; }
    public required string Address { get; set; }

    [Phone]
    public required string PhoneNumber { get; set; }
    public required string Status { get; set; } 

    public Role Role { get; set; }

    public required BankBranch BankBranch { get; set; }

    

}



