using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeHub.Utilities
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            // Use JwtRegisteredClaimNames.Sub to retrieve the user ID
            var claim = user.Claims.FirstOrDefault(c => c.Type == "UserId");
            
            if (claim != null && Guid.TryParse(claim.Value, out Guid userId))
            {
                return userId;
            }
            throw new Exception("User ID not found in claims.");
        }

        public static string GetUserEmail(ClaimsPrincipal user)
        {
            // Retrieve the user's email from the claims
            var claim = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
            if (claim != null)
            {
                return claim.Value;
            }
            throw new Exception("User email not found in claims.");
        }

        public static IEnumerable<string> GetUserRoles(ClaimsPrincipal user)
        {
            // Retrieve the user's roles from the claims
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
        }
    }
}