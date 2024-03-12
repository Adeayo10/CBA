using API.Models.PasswordModel;
using CBA.Context;
using CBA.Enums;
using CBA.Models;
using CBA.Models.AuthModel;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Web;

namespace CBA.Services;

public class UserService : IUserService
{
    private readonly ITokenService _tokenService;
    private readonly IValidator<ApplicationUser> _validatorService;
    private readonly IPasswordService _passwordService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailService _emailService;
    private readonly UserDataContext _context;

    public UserService(UserDataContext context, IPasswordService passwordService,
    ITokenService tokenService, UserManager<ApplicationUser> userManager, ILogger<UserService> logger,
    IValidator<ApplicationUser> validationService, IEmailService emailService)
    {
        _tokenService = tokenService;
        _passwordService = passwordService;
        _userManager = userManager;
        _validatorService = validationService;
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _logger.LogInformation("UserService constructor called");
    }
    public async Task<RegistrationResponse> AddUser(UserProfileDTO user)
    {
        _logger.LogInformation("AddUser method called");
        var userExist = await _userManager.FindByEmailAsync(user.Email);
        if (userExist != null)
        {
            _logger.LogInformation("User already exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User already exist"
                }
            };
        }

        var hashPassword = _passwordService.HashPassword(user.Password);
        _logger.LogInformation($"Hashed password: {hashPassword}");

        var newUser = CreateUserDetails(user, hashPassword);
        
        var validUserDetails = await ValidateUserDetails(newUser);
        _logger.LogInformation($"User details isValid: {validUserDetails.Success}");
        if (!validUserDetails.Success)
        {
            var errorMessage = validUserDetails.Errors.FirstOrDefault();
            _logger.LogError($"Error occurred validating userDetails: {errorMessage}");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    errorMessage!
                }
            };
        }
        var bankBranch = CreateUserBranchDetails(user.BankBranch, newUser);
        var result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)
        {
            _logger.LogError($"Error occurred in creating user: {result.Errors.FirstOrDefault()?.Description}");
            return new RegistrationResponse
            {
                Success = false,
                Errors = new List<string>()
                {
                    result.Errors.FirstOrDefault()?.Description!
                }
            };
        }
        await _context.BankBranch.AddAsync(bankBranch);
        await _context.SaveChangesAsync();

        await CreateBranchUser(bankBranch, newUser);
        _logger.LogInformation($"BranchUser created successfully");

        //await SendUserConfirmationEmail(newUser);

        await SendUserLoginCredentialsEmail(newUser);
        _logger.LogInformation($"Email sent successfully");

        return new RegistrationResponse()
        {
            Success = true,
            Message = "User created successfully"
        };
    }
    public async Task<LoginResponse> LoginUser(UserLoginRequest user)
    {

        var userExist = await _userManager.FindByEmailAsync(user.Email);

        if (userExist == null)
        {
            _logger.LogError($"Error occurred in Login method: User does not exist");
            return new LoginResponse()
            {
                Success = false,
                Errors = new List<string>() { "User does not exist" },
                Message = "User does not exist"
            };
        }

        string passwordHash = userExist.PasswordHash;
        bool validPassword = _passwordService.IsValidPassword(user.Password, passwordHash);

        _logger.LogInformation($"Password is valid: {validPassword}");

        if (validPassword)
        {
            _logger.LogInformation($"User {userExist.UserName} logged in successfully");
            await SendUserTokenEmail(userExist);
            return new LoginResponse
            {
                Success = true,
                Message = "Check your email for verification Token",
            };
            
        }
        else
        {
            _logger.LogError($"Error occurred in Login method: Invalid password");
            return new LoginResponse
            {
                Success = false,
                Errors = new List<string>() { "Invalid password" },
                Message = "Invalid password"
            };
        }
    }
    
    public async Task<LogoutResponse> LogoutUser(string userName)
    {
        var result = await _tokenService.RevokeToken(userName);
        if (result.Success)
        {
            _logger.LogInformation("User logged out successfully");
            return new LogoutResponse()
            {
                Message = "User logged out successfully",
                StatusCode = "200",
                Success = true
            };
        }
        else
        {
            _logger.LogError("Error occurred in LogoutUser method: User not logged out");
            return new LogoutResponse()
            {
                Message = "User not logged out",
                StatusCode = "400",
                Success = false
            };
        }
    }
    
    public async Task<RegistrationResponse> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogError($"Error occurred in ConfirmEmail method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} confirmed email successfully");
            return new RegistrationResponse()
            {
                Success = true,
                Message = "Email confirmed successfully"
            };
        }
        else
        {
            _logger.LogError($"Error occurred in ConfirmEmail method: Email not confirmed");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Email not confirmed"
                }
            };
        }
    }
    public async Task<RegistrationResponse> ActivateUser(ActivateUserDTO inactiveUser)
    {
        var userId = inactiveUser.UserId;
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            _logger.LogError($"Error occurred in ActivateUser method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }
        user.Status = "Active";
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} activated successfully");
            return new RegistrationResponse()
            {
                Success = true,
                Message = "User activated successfully"
            };
        }
        else
        {
            _logger.LogError($"Error occurred in ActivateUser method: User not activated");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User not activated"
                }
            };
        }
    }
    public async Task<RegistrationResponse> DeActivateUser(DeActivateUserDTO activeUser)
    {
        var userId = activeUser.UserId;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogError($"Error occurred in DeActivateUser method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }
        user.Status = "Deactivated";
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} deactivated successfully");
            return new RegistrationResponse()
            {
                Success = true,
                Message = "User deactivated successfully"
            };
        }
        else
        {
            _logger.LogError($"Error occurred in DeActivateUser method: User not deactivated");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User not deactivated"
                }
            };
        }
    }
    public async Task<RegistrationResponse> ForgetPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogError($"Error occurred in ForgetPassword method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }

        await SendUserResetPasswordEmail(user);
        _logger.LogInformation($"Reset password token sent to {email}");
        return new RegistrationResponse()
        {
            Success = true,
            Message = "Reset password token sent successfully"
        };
    }
    public async Task<RegistrationResponse> ChangePassword(ChangePasswordDTO changePassword)
    {
        var user = await _userManager.FindByEmailAsync(changePassword.Email!);
        if (user == null)
        {
            _logger.LogError($"Error occurred in ChangePassword method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }

        var currentDBPassword = _context.Users.FirstOrDefault(x => x.Email == changePassword.Email)?.PasswordHash;
        var validPassword = _passwordService.IsValidPassword(changePassword.CurrentPassword!, currentDBPassword!);
        if (!validPassword)
        {
            _logger.LogError($"Error occurred in ChangePassword method: Invalid password");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid password"
                }
            };
        }
        user.PasswordHash = _passwordService.HashPassword(changePassword.NewPassword!);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} changed password successfully");
            return new RegistrationResponse()
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
        else
        {
            _logger.LogError($"Error occurred in ChangePassword method: Password not changed");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Password not changed"
                }
            };
        }
    }
    public async Task<RegistrationResponse> ResetPassword(ResetPasswordDTO resetPassword)
    {
        _logger.LogInformation($"ResetPassword DTO => id: {resetPassword.UserId}, token: {resetPassword.Token}, password: {resetPassword.Password}");
        var user = await _userManager.FindByIdAsync(resetPassword.UserId!);
        _logger.LogInformation($"User => email: {user!.Email}, the rest: {user.PasswordHash}");
        if (user == null)
        {
            _logger.LogError($"Error occurred in ResetPassword method: User does not exist");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }

        var decodedToken = HttpUtility.UrlDecode(resetPassword.Token!);
        var newPassword = _passwordService.HashPassword(resetPassword.Password!);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

        user.PasswordHash = newPassword;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Result: {result}");
        
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} reset password successfully");
            _logger.LogInformation($"User => email: {user.Email}, the rest: {user.PasswordHash}");
            return new RegistrationResponse()
            {
                Success = true,
                Message = "Password reset successfully"
            };
        }
        else
        {
            _logger.LogError($"Error occurred in ResetPassword method: Password not reset");
            return new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    $"Password not reset: {result.Errors.FirstOrDefault()?.Description}"
                }
            };
        }
    }
    public async Task<RegistrationResponse> UpdateUser(UserUpdateDTO user)
    {
        var userExist = await _userManager.FindByIdAsync(user.Id.ToString());
        if (userExist is null)
        {
            _logger.LogError($"Error occurred in UpdateUser method: User does not exist");
            return new RegistrationResponse
            {
                Errors = new List<string>() { "User was not found" },
                Success = false
            };
        }

        _logger.LogInformation($"User {userExist?.UserName} found");
        if (userExist.Status == "Deactivated")
        {
            _logger.LogError($"Error occurred in UpdateUser method: User is deactivated");
            return new RegistrationResponse
            {
                Errors = new List<string>() { "Deactivated user cannot be updated" },
                Success = false
            };
        }

        var isUserUpdated = await UpdatedUser(userExist!, user);

        var bankBranchUpdateDetails = CreateUserBranchDetails(user.BankBranch!, userExist!);
        _logger.LogInformation($"BankBranch {bankBranchUpdateDetails.Name} found");

        await UpdateBankBranch(bankBranchUpdateDetails, userExist!);
        _logger.LogInformation($"BankBranch {bankBranchUpdateDetails.Name} updated successfully");

        if (isUserUpdated.Succeeded)
        {
            _logger.LogInformation($"User {userExist?.UserName} updated successfully");
            return new RegistrationResponse()
            {
                Success = true,
                Errors = null,
                Message = "User updated successfully!"
            };
        }
        else
        {
            var errorDescriptions = isUserUpdated.Errors.Select(x => x.Description).ToList();
            _logger.LogError($"Error occurred in updating user: {errorDescriptions.FirstOrDefault()}");
            return new RegistrationResponse
            {
                Success = false,
                Errors = errorDescriptions
            };
        }
    }
    public async Task<UserResponse> GetUser(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogError($"Error occurred in GetUser method: User does not exist");
            return new UserResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }
        _logger.LogInformation($"User {user.UserName} found");

        var BankBranch = _context.BankBranch.FirstOrDefault(x => x.UserId == user.Id);
        return new UserResponse()
        {
            Success = true,
            Users = new List<ApplicationUser>()
            {
                user
            },
            UserBranch = new List<BankBranch>()
            {
                BankBranch!
            }
        };
    }
    public async Task<UserResponse> GetAllUsers(int pageNumber, int pageSize)
    {
        var users = await _userManager.Users.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var usersDT0 = users.Select(x => new ApplicationUser
        {
            Id = x.Id,
            Email = x.Email,
            UserName = x.UserName,
            Role = x.Role,
            PhoneNumber = x.PhoneNumber,
            FullName = x.FullName,
            Address = x.Address,
            Status = x.Status,

        }).ToList();
        var bankBranch = await _context.BankBranch.ToListAsync();
        var bankBranchDTO = bankBranch.Select(x => new BankBranch
        {
            UserId = x.UserId,
            Name = x.Name,
            Region = x.Region,
            Code = x.Code,
            Description = x.Description
        }).ToList();
        if (users == null)
        {
            _logger.LogError($"Error occurred in GetAllUsers method: User does not exist");
            return new UserResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "User does not exist"
                }
            };
        }
        _logger.LogInformation($" {users.Count} User found");
        return new UserResponse()
        {
            Success = true,
            Users = usersDT0,
            UserBranch = bankBranchDTO

        };
    }
    private static ApplicationUser CreateUserDetails(UserProfileDTO user, string hashPassword)
    {
        var newUser = new ApplicationUser()
        {
            Email = user.Email,
            UserName = user.UserName,
            Password = user.Password,
            Role = user.Role,
            PasswordHash = hashPassword, // Set the PasswordHash property
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Address = user.Address,
            Status = user.Status,
        };

        return newUser;
    }
    private static BankBranch CreateUserBranchDetails(BankBranch branch, ApplicationUser user)
    {
        var bankBranch = new BankBranch()
        {
            UserId = user.Id,
            Name = branch.Name,
            Region = branch.Region,
            Code = GenerateCode(branch.Region, branch.Name),
            Description = branch.Description
        };
        return bankBranch;
    }
    private static string GenerateCode(string region, string branch)
    {
        var random = new Random();
        var code = $"{region[0].ToString().ToUpper()}{branch[0].ToString().ToUpper()}{random.Next(0, 9)}";
        return code;
    }
    private async Task CreateBranchUser(BankBranch bankBranch, ApplicationUser user)
    {
        var branchUser = new BranchUser()
        {
            UserId = user.Id,
            BranchId = bankBranch.Id
        };
        await _context.BranchUser.AddAsync(branchUser);
        await _context.SaveChangesAsync();
    }
    private async Task SendUserLoginCredentialsEmail(ApplicationUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = UrlEncoder.Default.Encode(token);
        var resetPasswordLink = $"http://localhost:5173/reset-password?userId={user.Id}&token={encodedToken}";
        var message = new Message(new string[] { user.Email! }, "User Confirmation", $"Hello {user.FullName}, <br/> <br/> You have successfully created an account with CBA. <br/> <br/> Your username is: {user.UserName} <br/> <br/> Your password is: {user.Password} <br/> <br/> To reset your password <a href=\"{resetPasswordLink}\" target=\"_blank\">Click Here</a> <br/> <br/> Thank you. ");
        await _emailService.SendEmail(message);
    }
    private async Task SendUserResetPasswordEmail(ApplicationUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = UrlEncoder.Default.Encode(token);
        var callback = $"http://localhost:5173/reset-password?userId={user.Id}&token={encodedToken}";
        var message = new Message(new string[] { user.Email! }, "Reset password token", $"Hello {user.UserName},<br/><br/> To reset your password <a href=\"{callback}\" target=\"_blank\">Click Here</a> <br/><br/>Thank you");
        await _emailService.SendEmail(message);
    }
    private async Task SendUserConfirmationEmail(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = UrlEncoder.Default.Encode(token);
        var callback = $"http://localhost:5173/confirm-email?userId={user.Id}&token={encodedToken}";
        var message = new Message(new string[] { user.Email! }, "Confirm Email", $"Hello {user.UserName},<br/><br/> Please confirm your account by clicking <a href=\"{callback}\" target=\"_blank\">here</a> <br/><br/>Thank you");
        await _emailService.SendEmail(message);
    }
    private async Task SendUserTokenEmail(ApplicationUser user)
    {
        var token = GenerateSixDigitToken();
        var tokenExpiry = DateTime.Now.AddMinutes(5);
        var callback = $"http://localhost:5173/verify-token?userId={user.Id}&token={token}";
        var message = new Message(new string[] { user.Email! }, "Enter Token", $"Hello {user.UserName},<br/><br/> Please enter the following six-digit token: {token} <br/><br/> click <a href=\"{callback}\" target=\"_blank\">here</a> to complete your login request <br/><br/> token expires in {tokenExpiry}, <br/><br/>Thank you");
        await _emailService.SendEmail(message);
    }
    public async Task<AuthResult> ConfirmUserToken(ApplicationUser user, string token)
    {
        var result = await _userManager.VerifyUserTokenAsync(user, "Default", "TwoFactor", token);
        if (result)
        {
            _logger.LogInformation($"User {user.UserName} verified token successfully");
            var generateUserToken = _tokenService.GenerateTokens(user);
            _logger.LogInformation($"Token generated");
            return new AuthResult
            {
                Success = true,
                Token = generateUserToken.Result.Token,
                RefreshToken = generateUserToken.Result.RefreshToken,
                ExpiryDate = generateUserToken.Result.ExpiryDate
            };
        }
        else
        {
            _logger.LogError($"Error occurred in ConfirmUserToken method: Token not verified");
            return new AuthResult
            {
                Success = false,
                Errors = new List<string>() { "Token not verified" }
            };
        }
    }

    private static string GenerateSixDigitToken()
    {
        var random = new Random();
        var token = random.Next(100000, 999999).ToString();
        return token;
    }
    private async Task<UserResponse> ValidateUserDetails(ApplicationUser user)
    {
        var validationResult = await _validatorService.ValidateAsync(user);
        if (validationResult.IsValid)
        {
            return new UserResponse
            {
                Success = true
            };
        }
        var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        return new UserResponse
        {
            Success = false,
            Errors = errors
        };
    }
    private async Task<IdentityResult> UpdatedUser(ApplicationUser userExist, UserUpdateDTO user)
    {
        userExist.Email = user.Email;
        userExist.UserName = user.UserName;
        userExist.Password = userExist.PasswordHash;
        userExist.Role = user.Role;
        userExist.PhoneNumber = user.PhoneNumber;
        userExist.FullName = user.FullName;
        userExist.Address = user.Address;
        userExist.Status = user.Status;
        var result = await _userManager.UpdateAsync(userExist);
        return result;
    }
    private async Task UpdateBankBranch(BankBranch UpdatedBankBranch, ApplicationUser userExist)
    {
        var bankBranch = _context.BankBranch.FirstOrDefault(x => x.UserId == userExist.Id);
        bankBranch!.Name = UpdatedBankBranch.Name;
        bankBranch.Region = UpdatedBankBranch.Region;
        bankBranch.Code = UpdatedBankBranch.Code;
        bankBranch.Description = UpdatedBankBranch.Description;
        await _context.SaveChangesAsync();
    }
    public object GetUserRoles()
    {
        var roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToList();
        var mappedRoles = roles.Select(x => new
        {
            Id = (int)x,
            Name = x.ToString()
        });
        return mappedRoles;
    }

    /*private async Task UpdateBranchUser(BankBranch UpdatedBankBranch, ApplicationUser userExist)
        {
            var branchUser = _context.BranchUser.FirstOrDefault(x => x.UserId == userExist.Id);
            branchUser.BranchId = UpdatedBankBranch.Id;
            await _context.SaveChangesAsync();
        }*/
}