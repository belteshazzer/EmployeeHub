using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;

namespace EmployeeHub.Services.AuthServices
{
    public interface IAuthService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(User user, string password);
        Task<bool> VerifyEmailAsync(string email, string token);
        Task<LoginResponse> LoginUserAsync(string email, string password);
        Task<bool> ResendVerificationEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> SendPasswordResetEmailAsync(string email);
        Task LogoutUserAsync();
        Task<bool> DeleteUserAsync(string email);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
    }
}