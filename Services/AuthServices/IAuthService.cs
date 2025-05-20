using EmployeeHub.Models.Dtos;

namespace EmployeeHub.Services.AuthServices
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(string username, string password);
        Task<LoginResponse> LoginAsync(string username, string password);
        Task<bool> LogoutAsync(string token);
        Task<bool> ResetPasswordAsync(string username, string newPassword);
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
    }
}