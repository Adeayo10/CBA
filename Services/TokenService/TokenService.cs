using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using CBA.Models.TokenModel;
using CBA.Context;
using CBA.Models;
using CBA.Models.AuthModel;
using Microsoft.EntityFrameworkCore;




namespace CBA.Services;
public class TokenService : ITokenService
{
    public readonly UserDataContext _context;
    private readonly ILogger<TokenService> _logger;

    private readonly TokenValidationParameters _tokenValidationParameters;

    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(UserDataContext context, ILogger<TokenService> logger, TokenValidationParameters tokenValidationParameters, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _tokenValidationParameters = tokenValidationParameters;
        _userManager = userManager;
        _logger.LogInformation("TokenService constructor called");
    }
    public async Task<AuthResult> GenerateTokensAsync(ApplicationUser user)
    {
        _logger.LogInformation("GenerateToken method called");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = CreateJwtToken(CreateClaimsIdentity(user), CreateSigningCredentials(), DateTime.Now.AddMinutes(5));
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        var refreshToken = await CreateRefreshTokenAsync(user, token);
        _logger.LogInformation("Token generated successfully");

        return new AuthResult()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            /*ExpiryDate = tokenOptions.ValidTo.ToLocalTime(),*/
            ExpiryDate = token.ValidTo.ToLocalTime(),
        };
    }
    public async Task<AuthResult> VerifyTokenAsync(TokenRequest tokenRequest)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

            _logger.LogInformation("Token validation started");
            _logger.LogInformation($"Token validation result: {principal.Identity?.IsAuthenticated} with Parameters:{validatedToken}, {validatedToken?.ValidTo}, {validatedToken?.ValidFrom}");


            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var isValidJwtAlgorithm = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                _logger.LogInformation($"Token security algorithm is valid: {isValidJwtAlgorithm}");
                if (!isValidJwtAlgorithm)
                {
                    _logger.LogError($"Error occured in VerifyToken method: Invalid security algorithm");
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Invalid security algorithm" },
                        Success = false
                    };
                }
            }

            var isTokenExpired = validatedToken.ValidTo < DateTime.Now;
            _logger.LogInformation($"Token is expired: {isTokenExpired}");

            if (isTokenExpired)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Token has expired" },
                    Success = false
                };
            }

            var storedRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
            _logger.LogInformation("Refresh token found in db: {result}", storedRefreshToken != null);

            if (storedRefreshToken == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "refresh token doesnt exist" },
                    Success = false
                };
            }

            if (DateTime.Now > storedRefreshToken.ExpiryDate)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "token has expired, user needs to relogin" },
                    Success = false
                };
            }
            if (storedRefreshToken.IsRevoked)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "token has been revoked" },
                    Success = false
                };
            }
            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // check the id that the recieved token has against the id saved in the db
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "the token doesn't match the saved token" },
                    Success = false
                };
            }
            storedRefreshToken.IsUsed = true;
            _context.RefreshToken.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId!);

            var generatedToken = await GenerateTokensAsync(dbUser!);
            return generatedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured in VerifyToken method: {ex.Message}");
            return ex switch
            {
                SecurityTokenExpiredException => new AuthResult()
                {
                    Errors = new List<string>() { "Token has expired..., user needs to relogin" },
                    Success = false
                },
                SecurityTokenException => new AuthResult()
                {
                    Errors = new List<string>() { "Token has been tempered" },
                    Success = false
                },
                _ => new AuthResult()
                {
                    Errors = new List<string>() { "Something went wrong" },
                    Success = false
                }
            };
        }
    }
    private async Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user, SecurityToken token)
    {
        var refreshToken = new RefreshToken()
        {
            /*JwtId = tokenOptions.Id,*/
            JwtId = token.Id,
            IsUsed = false,
            UserId = user.Id,
            AddedDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddMinutes(30),
            IsRevoked = false,
            Token = RandomString(25) + Guid.NewGuid()
        };

        await _context.RefreshToken.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;

    }
    public async Task<LogoutResponse>  RevokeTokenAsync(string userName)
    {
        _logger.LogInformation("RevokeToken method called");
       
        var user = await _userManager.FindByNameAsync(userName ?? string.Empty);
        if (user == null)
        {
            _logger.LogError($"Error occurred in RevokeToken method: User does not exist");
            return new LogoutResponse()
            {
                Message = "User does not exist",
                StatusCode = "404",
                Success = false
            }; 
        }
        var refreshToken = _context.RefreshToken.FirstOrDefault(u => u.UserId == user.Id);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.IsUsed = true;
            _context.RefreshToken.RemoveRange(_context.RefreshToken.Where(u => u.UserId == user.Id));
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation($"Refresh token revoked for user: {userName}");
        await _userManager.UpdateSecurityStampAsync(user);    
        await _userManager.UpdateAsync(user);
        _logger.LogInformation($"User updated");
        return new LogoutResponse()
        {
            Message = "User logged out successfully",
            StatusCode = "200",
            Success = true
        };
    }
    private static string RandomString(int length)
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private SecurityTokenDescriptor CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = credentials,
            Issuer = _tokenValidationParameters.ValidIssuer,
            Audience = _tokenValidationParameters.ValidAudience,
        };
    private SigningCredentials CreateSigningCredentials()
    {
        var secretKey = _tokenValidationParameters.IssuerSigningKey;
        return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }
    private static List<Claim> CreateClaimsIdentity(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
                // the JTI is used for our refresh token
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        return claims;
    }
}

