using Microsoft.AspNetCore.Identity;
using EmployeeHub.Common.EmailSender;
using EmployeeHub.Models.Entities;
using EmployeeHub.Common.Exceptions;
using EmployeeHub.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EmployeeHub.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly RoleManager<Roles> _roleManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Roles> roleManager, IEmailSender emailSender, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
            .Include(u => u.Department)
            .Include(u => u.Role)
            .ToListAsync();

            if (users == null || !users.Any())
            {
            throw new NotFoundException("No users found.");
            }
            return users;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new ConflictException("A user with this email already exists.");
            }

            // Ensure username is set
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = user.Email;
            }

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign default "Officer" role
                var officerRole = await _roleManager.FindByNameAsync("Officer") ?? throw new NotFoundException("Role 'Officer' not found");
                await _userManager.AddToRoleAsync(user, officerRole.Name);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _logger.LogInformation("token: {token}", token);
                await SendConfirmationEmail(user.Email, token);
                return user;
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User registration failed: {Errors}", errors);
                throw new Exception("User registration failed: " + errors);
            }
        }

        private async Task SendConfirmationEmail(string email, string token)
        {
            var confirmationLink = $"http://localhost:5039/api/Auth/confirm-email?email={email}&token={Uri.EscapeDataString(token)}";
            _logger.LogInformation("confirmationLink: {confirmationLink}", confirmationLink);

            var subject = "Password Reset Request";
            var body = string.Format(EmailTemplates.ConfirmEmailTemplate, confirmationLink);
            var emailSent = await _emailSender.SendEmail(email, subject, body);
            if (!emailSent)
            {
                throw new Exception("Failed to send confirmation email, please try again");
            }
        }

       public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("User not found");
            if (user.EmailConfirmed)
            {
                throw new Exception("Email is already confirmed"); 
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Email confirmation failed: {Errors}", errors);
                throw new Exception("Email confirmation failed: " + errors);
            }

            var role = await _roleManager.FindByNameAsync("User") ?? throw new NotFoundException("Role not found");
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                throw new Exception("Role name is invalid");
            }
            await _userManager.AddToRoleAsync(user, role.Name);

            return true;
        }

        public async Task<LoginResponse> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.UserName))
        {
            throw new Exception("User data is invalid. Email or UserName is missing.");
        }
        _logger.LogInformation("User Email: {Email}, UserName: {UserName}", user?.Email, user?.UserName);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            { 
                var token = GenerateJwtTokenAsync(user);              
                var loginResponse = new LoginResponse
                {
                    Token = token,
                    RefreshToken = user.RefreshToken,
                }; 
                return loginResponse;
            }
            else if (await _userManager.IsLockedOutAsync(user))
            {
                throw new AccessDeniedException("User account is locked out. Please contact support."); 
            }
            else if (result.IsNotAllowed)
            {
                throw new UnauthorizedException("User is not allowed to login. Please verify your email first.");
            }
            else 
            {
                throw new BadRequestException("Invalid login attempt. Please check your credentials."); 
            }
        }

        public string GenerateJwtTokenAsync(User user)
        {
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            _logger.LogInformation("Generating JWT token for user: {userId}", user.Id);

            var jwtConfig = _configuration.GetSection("Jwt");
            var key = jwtConfig["Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Department", user.Department?.Name ?? "Not specified"),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig["ExpirationMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtConfig["Issuer"],
                Audience = jwtConfig["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("Generated JWT token for user {Email}: {Token}", user.Email, tokenString);

            return tokenString;

        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException("User not found"); // User does not exist
            }
            else if (user.EmailConfirmed)
            {
                // Email is already confirmed, no need to resend
                throw new Exception("Email is already confirmed"); // Email is already confirmed
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogInformation("Resending confirmation email to {Email} with token: {Token}", email, token);
            await SendConfirmationEmail(email, token);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Password reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else {
                return true; 
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result ?? throw new NotFoundException("User not found");

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            var resetLink = $"https://localhost:5001/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            var subject = "Password Reset Request";
            var body = string.Format(EmailTemplates.PasswordResetTemplate, resetLink);
            var isSent = await _emailSender.SendEmail(email, subject, body);
            if (!isSent)
            {
                throw new Exception("Failed to send password reset email, please try again");
            }
            return true;
        }
        
        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Password change failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return true;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("User deletion failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return true;
        }
    }
}