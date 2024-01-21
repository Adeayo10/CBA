using CBA.Context;
using CBA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CBA.Models.AuthModel;
using CBA.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;


namespace CBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IValidator<ApplicationUser> _validatorService;
        private readonly IPasswordService _passwordService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;
        private readonly UserDataContext _context;

        public AuthController(UserDataContext context, IPasswordService passwordService, ITokenService tokenService, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger, IValidator<ApplicationUser> validationService, IEmailService emailService)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userManager = userManager;
            _validatorService = validationService;
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _logger.LogInformation("AuthController constructor called");
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            try
            {
                _logger.LogInformation("Login method called");

                if (user is null)
                {
                    return BadRequest("Invalid client request");
                }

                var userExist = await _userManager.FindByEmailAsync(user.Email ?? string.Empty);
                if (userExist == null)
                {
                    _logger.LogError($"Error occurred in Login method: User does not exist");
                    return BadRequest(new { error = "User does not exist" });
                }

                string passwordhash = userExist.PasswordHash;
                var validPassword = _passwordService.IsValidPassword(user.Password, passwordhash);

                _logger.LogInformation($"Password is valid: {validPassword}");
                if (validPassword)
                {
                    var token = _tokenService.GenerateTokens(userExist);
                    _logger.LogInformation($"Token generated");

                    return Ok(new LoginResponse
                    {
                        Success = true,
                        Token = token.Result.Token,
                        RefreshToken = token.Result.RefreshToken,
                        Message = "Login successful!",
                        ExpiryDate = token.Result.ExpiryDate
                    });
                }
                else
                {
                    _logger.LogError($"Error occurred in Login method: Invalid password");
                    return BadRequest(new { error = "Invalid password" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login method");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserProfile user)
        {
            try
            {
                _logger.LogInformation("Register method called");

                var userExist = await _userManager.FindByEmailAsync(user.Email ?? string.Empty);
                if (userExist != null)
                {
                    _logger.LogError("Error occurred in Register method: User already exists");
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string>() { "User already exists" },
                        Success = false
                    });
                }

                var hashPassword = _passwordService.HashPassword(user.Password);
                _logger.LogInformation($"Hashed password: {hashPassword}");

                var newUser = CreateUserDetails(user, hashPassword);

                var validUserDetails = await _validatorService.ValidateAsync(newUser);
                _logger.LogInformation($"User details isValid: {validUserDetails.IsValid}");

                if (!validUserDetails.IsValid)
                {
                    var errorMessage = validUserDetails.Errors.FirstOrDefault()?.ErrorMessage;
                    _logger.LogError($"Error occurred validating userDetails: {errorMessage}");
                    return BadRequest(new { error = errorMessage });
                }

                var bankBranch = CreateUserBranchDetails(user.BankBranch, newUser);
                var result = await _userManager.CreateAsync(newUser, user.Password);

                await _context.BankBranch.AddAsync(bankBranch);
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {newUser.UserName} created a new account.");

                    var message = new Message(new string[] { newUser.Email! }, " User Information", $"Hello {newUser.FullName}, <br/> <br/> You have successfully created an account with CBA. <br/> <br/> Your username is: {newUser.UserName} <br/> <br/> Your password is: {user.Password} <br/> <br/> Thank you. ");
                    await _emailService.SendEmail(message);
                    _logger.LogInformation($"Email sent successfully");

                    return Ok(new RegistrationResponse
                    {
                        Success = true,
                        Errors = null,
                        Message = "User created successfully!"
                    });
                }
                else
                {
                    var errorDescriptions = result.Errors.Select(x => x.Description).ToList();
                    _logger.LogError($"Error occurred in creating user: {errorDescriptions.FirstOrDefault()}");
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Errors = errorDescriptions
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register method");
                return StatusCode(500, new AuthResult
                {
                    Errors = new List<string>() { ex.Message },
                    Success = false
                });
            }
        }

        private static string GenerateCode(string region, string branch)
        {
            var code = $"{region.Substring(0, 1).ToUpper()}{branch.Substring(0, 1).ToUpper()}{new Random().Next(0, 9)}";
            return code;
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

        private static ApplicationUser CreateUserDetails(UserProfile user, string hashPassword)
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
    }
}
