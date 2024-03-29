using FluentValidation;
using CBA.Models;
using System.Text.RegularExpressions;

namespace CBA.Services;
public  partial class ValidatorService : AbstractValidator<ApplicationUser>
{   
    public ValidatorService()
    {
        RuleFor(user => user.UserName).NotEmpty().MaximumLength(20).MinimumLength(6).WithMessage("Username cannot be empty");
        RuleFor(user => user.Email).EmailAddress().WithMessage("Please provide a valid email").NotEmpty().WithMessage("Email cannot be empty");
        RuleFor(user => user.Password).MinimumLength(6).MaximumLength(300).Must(HasValidPassword).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character");
        RuleFor(user => user.FullName).NotEmpty().MaximumLength(20).MinimumLength(6);
        RuleFor(user => user.Address).NotEmpty();
        RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Must(HasValidPhoneNumber!).WithMessage("Please provide a valid phone number");
        RuleFor(user => user.Status).NotEmpty();
        RuleFor(user => user.Role.ToString()).Must(HasValidRole).WithMessage("Invalid role");
    }

    private static bool HasValidRole(string role)
    {
        return role switch
        {
            "Admin" => true,
            "User" => true,
            "SuperAdmin" => true,
            "Manager" => true,
            _ => false
        };
    }
    private static bool HasValidPassword(string password)
    {
        var lowercase = MyRegex();
        var uppercase = MyRegex1(); 
        var digit = MyRegex2();
        var symbol = MyRegex3();
        return lowercase.IsMatch(password) && uppercase.IsMatch(password) && digit.IsMatch(password) && symbol.IsMatch(password);
    }
    private static bool HasValidPhoneNumber(string phoneNumber)
    {
        var regex = MyRegex4();
        return regex.IsMatch(phoneNumber);
    }

    // private static bool HasValidRole(Role role)
    // {
    //     return role switch
    //     {
    //         Role.Admin => true,
    //         Role.User => true,
    //         Role.SuperAdmin => true,
    //         Role.Manager => true,
    //         _ => false
    //     };
    // }


    [GeneratedRegex("[a-z]+")]
    private static partial Regex MyRegex();
    [GeneratedRegex("[A-Z]+")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("(\\d)+")]
    private static partial Regex MyRegex2();
    [GeneratedRegex("(\\W)+")]
    private static partial Regex MyRegex3();
    [GeneratedRegex("^(\\d{11})$")] //@"^234\+?[0-9]{0,13}$"
    private static partial Regex MyRegex4();
}
