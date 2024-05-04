using CBA.Models;
using Microsoft.AspNetCore.Mvc;
using CBA.Models.AuthModel;
using CBA.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using API.Models.PasswordModel;

namespace CBA.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;
    public AuthController(ILogger<AuthController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
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

            var result = await _userService.LoginUserAsync(user);

            if (result.Success)
            {
                return Ok(new LoginResponse
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message,
                    UserId = result.UserId
                });
            }
            else
            {
                return BadRequest(new LoginResponse
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Login method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("verify-token")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyToken([FromBody] LoginTokenDTO user, string token)
    {
        try
        {
            _logger.LogInformation("VerifyEmail method called");
            var result = await _userService.ConfirmUserTokenAsync(user, token);
            return Ok(new AuthResult
            {
                Success = result.Success,
                Errors = result.Errors,
                Message = result.Message,
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                ExpiryDate = result.ExpiryDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in VerifyEmail method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("resend-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendToken([FromBody] LoginTokenDTO user)
    {
        try
        {
            _logger.LogInformation("ResendToken method called");
            if (user is null)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            await _userService.ResendTokenAsync(user);
            return Ok(new AuthResult
            {
                Success = true,
                Errors = null,
                Message = "Token resent successfully!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ResendToken method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("Logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            _logger.LogInformation("Logout method called");
            var userName = User.Identity?.Name;
            var result = await _userService.LogoutUserAsync(userName);

            if (result.Success)
            {

                return NoContent();
            }
            else
            {
                var response = new LogoutResponse
                {
                    Message = result.Message,
                    StatusCode = result.StatusCode,
                    Success = result.Success
                };
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Logout method");
            var response = new LogoutResponse
            {
                Message = ex.Message,
                StatusCode = "500",
                Success = false
            };
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    [Route("Register")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> Register([FromBody] UserProfileDTO user)
    {
        try
        {
            _logger.LogInformation("Register method called");
            if (user is null)
            {
                _logger.LogError($"Error occurred in Register method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var results = await _userService.AddUserAsync(user);
            if (!results.Success)
            {
                return BadRequest(new AuthResult
                {
                    Errors = results.Errors,
                    Success = false
                });
            }
            return CreatedAtAction(nameof(Register), new AuthResult
            {
                Success = results.Success,
                Errors = results.Errors,
                Message = results.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Register method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { "message " },
                Success = false
            });
        }
    }

    [HttpPut]
    [Route("UpdateUser")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Update([FromBody] UserUpdateDTO user)
    {
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
            await _userService.UpdateUserAsync(user);
            return Ok(new AuthResult
            {
                Success = true,
                Errors = null,
                Message = "User updated successfully!"
            });
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

    [HttpGet]
    [Route("GetUser")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            // remove ID from the parameter since it is already in the route  
            var result = await _userService.GetUserAsync(id);
            return Ok(new UserResponse
            {
                Success = result.Success,
                Errors = result.Errors,
                Users = result.Users,
                UserBranch = result.UserBranch
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Get method");
            return StatusCode(500, new UserResponse
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpGet]
    [Route("GetAllUsers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber, int pageSize)
    {
        try
        {
            var result = await _userService.GetAllUsersAsync(pageNumber, pageSize);
            _logger.LogInformation("GetAll method called");
            return Ok(new UserResponse
            {
                Success = result.Success,
                Errors = result.Errors,
                Users = result.Users,
                UserBranch = result.UserBranch,
                TotalUsers = result.TotalUsers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAll method");
            return StatusCode(500, new UserResponse
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("forget-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDTO forgetPasswordDTO)
    {
        try
        {
            if (forgetPasswordDTO == null)
            {
                _logger.LogError($"Error occurred in ForgetPassword method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var result = await _userService.ForgetPasswordAsync(forgetPasswordDTO.Email!);
            if (result.Success)
            {
                return Ok(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = "Password reset link sent successfully!"
                });
            }
            else
            {
                return BadRequest(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = "Password reset link failed to send!"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ForgetPassword method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            if (changePasswordDTO == null)
            {
                _logger.LogError($"Error occurred in ChangePassword method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var result = await _userService.ChangePasswordAAsync(changePasswordDTO);
            if (result.Success)
            {
                return Ok(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ChangePassword method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
    {
        try
        {
            if (resetPasswordDTO == null)
            {
                _logger.LogError($"Error occurred in ResetPassword method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var result = await _userService.ResetPasswordAsync(resetPasswordDTO);
            if (result.Success)
            {
                return Ok(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new AuthResult
                {
                    Success = result.Success,
                    Errors = result.Errors,
                    Message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ResetPassword method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("ConfirmEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        try
        {
            _logger.LogInformation("ConfirmEmail method called");
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userId))
            {
                return NotFound();
            }
            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.Success)
            {
                return Ok(new AuthResult
                {
                    Success = true,
                    Errors = null,
                    Message = "Email confirmed successfully!"
                });
            }
            else
            {
                return BadRequest(new AuthResult
                {
                    Success = false,
                    Errors = result.Errors,
                    Message = "Email confirmation failed!"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ConfirmEmail method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpGet]
    [Route("get-roles")]
    [AllowAnonymous]
    public IActionResult GetRoles()
    {
        try
        {
            _logger.LogInformation("GetRoles method called");
            var result = _userService.GetUserRoles();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetRoles method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("deactivate-user")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> DeactivateUser([FromBody] DeActivateUserDTO user)
    {
        try
        {
            _logger.LogInformation("DeactivateUser method called");
            if (user is null)
            {
                _logger.LogError($"Error occurred in DeactivateUser method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var result = await _userService.DeActivateUserAsync(user);
            return Ok(new AuthResult
            {
                Success = result.Success,
                Errors = result.Errors,
                Message = result.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in DeactivateUser method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }

    [HttpPost]
    [Route("activate-user")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ActivateUser([FromBody] ActivateUserDTO user)
    {
        try
        {
            _logger.LogInformation("ActivateUser method called");
            if (user is null)
            {
                _logger.LogError($"Error occurred in ActivateUser method: User object is null");
                return BadRequest(new AuthResult
                {
                    Errors = new List<string>() { "User object is null" },
                    Success = false
                });
            }
            var result = await _userService.ActivateUserAsync(user);
            return Ok(new AuthResult
            {
                Success = result.Success,
                Errors = result.Errors,
                Message = result.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ActivateUser method");
            return StatusCode(500, new AuthResult
            {
                Errors = new List<string>() { ex.Message },
                Success = false
            });
        }
    }
}

