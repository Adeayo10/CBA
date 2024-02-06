using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CBA.Enums;
public enum Role
{
    None,
    Admin,
    User,
    SuperAdmin,
    Manager,
}