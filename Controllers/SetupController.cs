using Asp.Versioning;
using CBA.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CBA.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
    public class SetupController : ControllerBase
    {
        private readonly UserDataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        protected readonly ILogger<SetupController> _logger;

        public SetupController(
            UserDataContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ILogger<SetupController> logger)
        {   
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;

            _logger.LogInformation("SetupController constructor called");   
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist) {
                //create the roles and seed them to the database: Question 1
                var roleResult = await _roleManager.CreateAsync (new IdentityRole (roleName));

                if (roleResult.Succeeded) {
                    _logger.LogInformation (1, "Roles Added");
                    return Ok(new {result = $"Role {roleName} added successfully"});
                } else {
                    _logger.LogInformation (2, "Error");
                    return BadRequest(new {error = $"Issue adding the new {roleName} role"});
                }
            }

            return BadRequest(new {error = "Role already exist"});
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("AddUsers")]
        public async Task<IActionResult> CreateUser(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    PhoneNumber = "1234567890"

                };

                var passwordHash = _userManager.PasswordHasher.HashPassword(newUser, password);

                var result = await _userManager.CreateAsync(newUser, passwordHash);


                if(result.Succeeded)
                {
                    _logger.LogInformation (1, $"User {newUser.Email} created");
                    return Ok(new {result = $"User {newUser.Email} created"});
                }
                else
                {
                    _logger.LogInformation (1, $"Error: Unable to create user {newUser.Email}");
                    return BadRequest(new {error = $"Error: Unable to create user {newUser.Email}"});
                }
            }

            // User already exist
            return BadRequest(new {error = "User already exist"});
        }   

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (roleName == "Admin" || roleName == "user")
            {
                // Do nothing
            }
            else
            {
                return BadRequest(new {error = "Invalid role"});
            }

            if(user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if(result.Succeeded)
                {
                    _logger.LogInformation (1, $"User {user.Email} added to the {roleName} role");
                    return Ok(new {result = $"User {user.Email} added to the {roleName} role"});
                }
                else
                {
                    _logger.LogInformation (1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                    return BadRequest(new {error = $"Error: Unable to add user {user.Email} to the {roleName} role"});
                }
            }

            // User doesn't exist
            return BadRequest(new {error = "Unable to find user"});
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var roles = await _userManager.GetRolesAsync(user??new IdentityUser());
                return Ok(roles);
            }
            catch (Exception)
            {
                
                throw new Exception ("Error: Unable to find user");   
            }
     
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);

                if(result.Succeeded)
                {
                    _logger.LogInformation (1, $"User {user.Email} removed from the {roleName} role");
                    return Ok(new {result = $"User {user.Email} removed from the {roleName} role"});
                }
                else
                {
                    _logger.LogInformation (1, $"Error: Unable to removed user {user.Email} from the {roleName} role");
                    return BadRequest(new {error = $"Error: Unable to removed user {user.Email} from the {roleName} role"});
                }
            }

            // User doesn't exist
            return BadRequest(new {error = "Unable to find user"});
        }

    }