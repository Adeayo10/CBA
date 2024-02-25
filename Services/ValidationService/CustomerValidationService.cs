using FluentValidation;
using CBA.Models;
using System.Text.RegularExpressions;

namespace CBA.Services;
public partial class CustomerValidatorService : AbstractValidator<CustomerEntity>
{
    public CustomerValidatorService()
    {
        RuleFor(user => user.FullName).NotEmpty().MaximumLength(20).MinimumLength(6).WithMessage("FullName cannot be empty");
        RuleFor(user => user.Email).EmailAddress().WithMessage("Please provide a valid email").NotEmpty().WithMessage("Email cannot be empty");
        RuleFor(user => user.Address).NotEmpty();
        RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Must(HasValidPhoneNumber!).WithMessage("Please provide a valid phone number");
        RuleFor(user => user.Branch).NotEmpty().WithMessage("Branch cannot be empty");
        RuleFor(user => user.Status).NotEmpty().Must(HasValidStatus!).WithMessage("Status must be either Active or Inactive");
        RuleFor(user => user.AccountNumber).NotEmpty().WithMessage("Account number cannot be empty");
        RuleFor(user => user.AccountType).NotEmpty().WithMessage("Account type cannot be empty").Must(x => HasValidAccountType(x.ToString())).WithMessage("Account type must be either Savings or Current");
    }
    private static bool HasValidStatus(string? status)
    {
        var regex = MyRegex1();
        return regex.IsMatch(status!);
    }

    private static bool HasValidPhoneNumber(string phoneNumber)
    {
        var regex = MyRegex2();
        return regex.IsMatch(phoneNumber);
    }
    private static bool HasValidAccountType(string accountType)
    {
        return accountType == "Savings" || accountType == "Current";
    }

    [GeneratedRegex("^(\\d{11})$")] //@"^234\+?[0-9]{0,13}$"
    private static partial Regex MyRegex1();

    [GeneratedRegex("^Active|Deactivated$")]
    private static partial Regex MyRegex2();
}
