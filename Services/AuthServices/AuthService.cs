using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeHub.Services.AuthServices
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task<(IdentityResult, string)> RegisterAsync(RegistrationDto registrationDto)
        {
            var user = new User { UserName = registrationDto.Email, Email = registrationDto.Email };
            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (!result.Succeeded)
            {
            return (result, null);
            }

            // Generate a token (e.g., JWT or any other token logic)
            var token = GenerateToken(user);

            return (result, token);
        }

        private string GenerateToken(User user)
        {
            // Example token generation logic (replace with your actual implementation)
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}