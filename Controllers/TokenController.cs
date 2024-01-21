using CBA.Context;
using CBA.Services;
using CBA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CBA.Models.AuthModel;
using CBA.Models.TokenModel;
using FluentValidation;
using Microsoft.AspNetCore.Authorization; // for TokenValidationParameters

namespace CBA.Controllers;
[Route("api/[controller]")]

[ApiController]
public class TokenController : ControllerBase
{
    private readonly UserDataContext _context;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthController> _logger;
    public TokenController(UserDataContext context,  ITokenService tokenService, UserManager<ApplicationUser> userManager,ILogger<AuthController> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _userManager = userManager;
        _logger = logger;
        _logger.LogInformation("Token constructor called");
    }
    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("RefreshToken method called");
            var response = await  _tokenService.VerifyToken(tokenRequest);
            if (response == null)
            {
                _logger.LogError($"Error occured in RefreshToken method: Token is invalid");
                return BadRequest(new {message = "Invalid token"});
            }
            _logger.LogInformation($"Token verified");
            return Ok(new AuthResult
            {
                Token = response.Token,
                RefreshToken = response.RefreshToken,
                Success = true,
                Errors = null,
                ExpiryDate = response.ExpiryDate,
                Message = "Token refreshed successfully",
            });
        }
        _logger.LogError($"Error occured in RefreshToken method: Model state is invalid");
        return BadRequest(new AuthResult
        {
            Token = null,
            RefreshToken = null,
            Success = false,
            Errors = new List<string> {"Invalid payload"},
            ExpiryDate = null,
            Message = "Invalid payload",
        });
         
    }

    [HttpPost, Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")] 
    [Route("RevokeToken")]
    public async Task<IActionResult> RevokeToken()
    {
        _logger.LogInformation("RevokeToken method called");
        var userName = User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(userName??string.Empty);
        if (user == null)
        {
            _logger.LogError($"Error occured in RevokeToken method: User does not exist");
            return BadRequest();
            
        }
        _context.RefreshToken.RemoveRange(_context.RefreshToken.Where(u => u.UserId == user.Id));
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Refresh token revoked for user: {userName}");
        await _userManager.UpdateAsync(user);
        _logger.LogInformation($"User updated");


        return NoContent();
    }   
}