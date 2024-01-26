using CBA.Context;
using CBA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CBA.Models.AuthModel;
using CBA.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using AutoMapper;

namespace CBA.Controllers
{   
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        private readonly IMapper _mapper;

        public AuthController(UserDataContext context, IPasswordService passwordService, 
        ITokenService tokenService, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger,
        IValidator<ApplicationUser> validationService, IEmailService emailService, IMapper mapper)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userManager = userManager;
            _validatorService = validationService;
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
            _logger.LogInformation("AuthController constructor called");
        }

        [MapToApiVersion("1.0")]
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

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserProfileDTO user)
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

                    await SendUserConfirmationEmail(newUser);
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

        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("UpdateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UserUpdateDTO user, Guid id){
            try
            {
                if (user == null)
                {
                    _logger.LogError($"Error occurred in Update method: User object is null");
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string>() { "User object is null" },
                        Success = false
                    });
                }

                var userExist = await _userManager.FindByIdAsync(id.ToString());

                if (userExist == null)
                {
                    _logger.LogError($"Error occurred in Update method: User does not exist");
                    return NotFound(new AuthResult
                    {
                        Errors = new List<string>() { "User does not found" },
                        Success = false
                    });
                }
                _logger.LogInformation($"User {userExist.UserName} found");

                userExist.UserName = user.UserName;
                userExist.Email = user.Email;
                userExist.FullName = user.FullName;
                userExist.Address = user.Address;
                userExist.PhoneNumber = user.PhoneNumber;
                userExist.Status = user.Status;
                userExist.Role = user.Role;
                userExist.PasswordHash = userExist.PasswordHash;

                var UpdatedBankBranch = CreateUserBranchDetails(user.BankBranch, userExist);
                _logger.LogInformation($"BankBranch {UpdatedBankBranch.Name} found"); 

                _context.BankBranch.Update(UpdatedBankBranch);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"BankBranch {UpdatedBankBranch.Name} updated successfully");
        
                var result = await _userManager.UpdateAsync(userExist);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {userExist.UserName} updated successfully");
                    return Ok(new AuthResult
                    {
                        Success = true,
                        Errors = null,
                        Message = "User updated successfully!"
                    });
                }
                else
                {
                    var errorDescriptions = result.Errors.Select(x => x.Description).ToList();
                    _logger.LogError($"Error occurred in updating user: {errorDescriptions.FirstOrDefault()}");
                    return BadRequest(new AuthResult
                    {
                        Success = false,
                        Errors = errorDescriptions
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Update method");
                return StatusCode(500, new AuthResult
                {
                    Errors = new List<string>() { ex.Message },
                    Success = false
                });
            }

        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetUser")]
        [AllowAnonymous]
        public  IActionResult Get(Guid id)
        {
            try
            {
                var user = _userManager.FindByIdAsync(id.ToString());
                if (user is null)
                {
                    _logger.LogError($"Error occurred in Get method: User does not exist");
                    return NotFound(new AuthResult
                    {
                        Errors = new List<string>() { "User was not found" },
                        Success = false
                    });
                }
                _logger.LogInformation($"User {user.Result?.UserName} found");
                
                var BankBranch = _context.BankBranch.FirstOrDefault(x => x.UserId == user.Result.Id);
                return Ok( new {user.Result, BankBranch});
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Get method");
                return StatusCode(500, new AuthResult
                {
                    Errors = new List<string>() { ex.Message },
                    Success = false
                });
            }
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetAllUsers")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userManager.Users.ToList();
                if (users is null)
                {
                    _logger.LogError($"Error occurred in GetAll method: Users does not exist");
                    return NotFound(new AuthResult
                    {
                        Errors = new List<string>() { "Users was not found" },
                        Success = false
                    });
                }
                _logger.LogInformation($"Users found");
                
                var BankBranch = _context.BankBranch.ToList();
                return Ok( new {users, BankBranch});
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAll method");
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
    
        private async Task SendUserConfirmationEmail(ApplicationUser user)
        {
        var message = new Message(new string[] { user.Email! }, "User Confirmation", $"Hello {user.FullName}, <br/> <br/> You have successfully created an account with CBA. <br/> <br/> Your username is: {user.UserName} <br/> <br/> Your password is: {user.Password} <br/> <br/> Thank you. ");
         await _emailService.SendEmail(message);
        }
    
    }
}
