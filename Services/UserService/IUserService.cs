// namespace EmployeeHub.Services.UserService
// {
//     public interface IUserService
//     {
//         Task<User?> GetUserByIdAsync(Guid userId);
//         Task<User?> GetUserByEmailAsync(string email);
//         Task<IEnumerable<User>> GetAllUsersAsync();
//         Task<bool> CreateUserAsync(User user, string password);
//         Task<bool> UpdateUserAsync(User user);
//         Task<bool> DeleteUserAsync(Guid userId);
//         Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
//         Task<bool> ValidateUserCredentialsAsync(string email, string password);
//     }
// }