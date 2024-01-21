// using Xunit;
// using Moq;
// using Microsoft.AspNetCore.Mvc;
// using CBA.Controllers;
// using CBA.Models;
// using CBA.Services;
// using Microsoft.AspNetCore.Identity;
// using FluentValidation;
// using CBA.Context;
// using CBA.Models.TokenModel;
// using CBA.Models.AuthModel;

// namespace CBA.Tests.Controllers
// {
//     public class AuthControllerTests
//     {
//         private readonly AuthController _controller;
//         private readonly Mock<ITokenService> _tokenServiceMock;
//         private readonly Mock<IPasswordService> _passwordServiceMock;
//         private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
//         private readonly Mock<ILogger<AuthController>> _loggerMock;
//         private readonly Mock<IValidator<ApplicationUser>> _validatorServiceMock;
//         private readonly Mock<IEmailService> _emailServiceMock;
//         private readonly Mock<UserDataContext> _contextMock;

//         public AuthControllerTests()
//         {
//             _tokenServiceMock = new Mock<ITokenService>();
//             _passwordServiceMock = new Mock<IPasswordService>();
//             _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
//             _loggerMock = new Mock<ILogger<AuthController>>();
//             _validatorServiceMock = new Mock<IValidator<ApplicationUser>>();
//             _emailServiceMock = new Mock<IEmailService>();
//             _contextMock = new Mock<UserDataContext>();

//             _controller = new AuthController(
//                 _contextMock.Object,
//                 _passwordServiceMock.Object,
//                 _tokenServiceMock.Object,
//                 _userManagerMock.Object,
//                 _loggerMock.Object,
//                 _validatorServiceMock.Object,
//                 _emailServiceMock.Object
//             );
//         }

//         [Fact]
//         public async Task Login_WithValidCredentials_ReturnsOkResult()
//         {
//             // Arrange
//             var user = new UserLoginRequest
//             {
//                 Email = "test@example.com",
//                 Password = "password"
//             };

//             var userExist = new ApplicationUser
//             {
//                 Email = "test@example.com",
//                 PasswordHash = "hashed_password",
//                 FullName = "John Doe",
//                 Password = "password",
//                 Address = "123 Main St",
//                 Status = "Active"
//             };
            
//             _tokenServiceMock.Setup(x => x.GenerateTokens(userExist)).ReturnsAsync(new TokenResponse
//             {
//                 Token = "access_token",
//                 RefreshToken = "refresh_token",
//                 ExpiryDate = DateTime.Now.AddDays(1)
//             }).Verifiable();

//             // Act
//             var result = await _controller.Login(user);

//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
//             Assert.True(loginResponse.Success);
//             Assert.Equal("access_token", loginResponse.Token);
//             Assert.Equal("refresh_token", loginResponse.RefreshToken);
//             Assert.Equal("Login successful!", loginResponse.Message);
//             Assert.NotNull(loginResponse.ExpiryDate);
//         }

//         [Fact]
//         public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
//         {
//             // Arrange
//             var user = new UserLoginRequest
//             {
//                 Email = "test@example.com",
//                 Password = "password"
//             };

//             var userExist = new ApplicationUser
//             {
//                 Email = "test@example.com",
//                 PasswordHash = "hashed_password",
//                 FullName = "John Doe",
//                 Password = "password",
//                 Address = "123 Main St",
//                 Status = "Active"
//             };

//             _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(userExist);
//             _passwordServiceMock.Setup(x => x.IsValidPassword(user.Password, userExist.PasswordHash)).Returns(false);

//             // Act
//             var result = await _controller.Login(user);

//             // Assert
//             var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//             var errorResponse = Assert.IsType<AuthResult>(badRequestResult.Value);
//             Assert.Equal("Invalid password", errorResponse.Errors[0]);
//         }

//         [Fact]
//         public async Task Login_WithNullUser_ReturnsBadRequest()
//         {
//             // Arrange
//             UserLoginRequest user = null;

//             // Act
//             var result = await _controller.Login(user);

//             // Assert
//             var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//             var errorResponse = Assert.IsType<AuthResult>(badRequestResult.Value);
//             Assert.Equal("Invalid client request", errorResponse.Errors[0]);
//         }

//         // Add more test cases for the Register method and other methods in the AuthController class
//     }
// }