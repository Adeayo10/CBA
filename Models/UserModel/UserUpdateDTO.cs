using System.ComponentModel.DataAnnotations;
using CBA.Enums;

namespace CBA.Models;

public class UserUpdateDTO
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string FullName  { get; set; }
    public required string Address { get; set; }

    [Phone]
    public required string PhoneNumber { get; set; }
    public required string Status { get; set; } 

    public  Role Role { get; set; }

    public  BankBranch? BankBranch { get; set; }
}



