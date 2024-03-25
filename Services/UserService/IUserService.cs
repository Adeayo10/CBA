
using API.Models.PasswordModel;
using CBA.Models;
using CBA.Models.AuthModel;

namespace CBA.Services
{
    public interface IUserService
    {
        Task<RegistrationResponse> AddUserAsync(UserProfileDTO user);
        Task<LoginResponse> LoginUserAsync(UserLoginRequest user);
        Task<RegistrationResponse> ConfirmEmailAsync(string userId, string token);
        Task<RegistrationResponse> ForgetPasswordAsync(string email);
        Task<RegistrationResponse> ResetPasswordAsync(ResetPasswordDTO resetPassword);
        Task<RegistrationResponse> UpdateUserAsync(UserUpdateDTO user);

        Task<UserResponse> GetUserAsync(Guid userId);
        Task<UserResponse> GetAllUsersAsync(int pageNumber, int pageSize);
        object GetUserRoles();
        Task<RegistrationResponse> ChangePasswordAAsync(ChangePasswordDTO changePassword);
        Task<RegistrationResponse> DeActivateUserAsync(DeActivateUserDTO user);
        Task<RegistrationResponse> ActivateUserAsync(ActivateUserDTO user);
        Task<LogoutResponse> LogoutUserAsync(string userName);
        Task<AuthResult> ConfirmUserTokenAsync(LoginTokenDTO tokenUser, string token);
        Task ResendTokenAsync(LoginTokenDTO user);
    }
}