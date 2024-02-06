
using CBA.Models;
using CBA.Models.AuthModel;

namespace CBA.Services
{
    public interface IUserService
    {
        Task<RegistrationResponse> AddUser(UserProfileDTO user);
        Task<LoginResponse> LoginUser(UserLoginRequest user);
        Task<RegistrationResponse> ConfirmEmail(string userId, string token);
        Task<RegistrationResponse> ForgetPassword(string email);
        Task<RegistrationResponse> ResetPassword(ResetPasswordDTO resetPassword);
        Task<RegistrationResponse> UpdateUser(UserUpdateDTO user);

        Task<UserResponse> GetUser(Guid userId);
        Task<UserResponse> GetAllUsers();
    }
}