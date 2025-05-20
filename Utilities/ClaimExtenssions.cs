using System.Security.Claims;

namespace EmployeeHub.Utilities
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim != null && Guid.TryParse(claim.Value, out Guid applicantId))
            {
                return applicantId;
            }
            throw new Exception("User ID not found in claims.");
        }
    }
}