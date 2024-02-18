using System.ComponentModel.DataAnnotations.Schema;
using CBA.Enums;
using Microsoft.AspNetCore.Identity;

namespace CBA.Models;
public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
    public required string FullName  { get; set; }
    [NotMapped]
    public  string Password { get; set; } 
    public new virtual string PasswordHash { get; set; }
    public required string Address { get; set; }
    public required string Status { get; set; } 
    public int BankBranch { get; set; }

    
}