using Microsoft.AspNetCore.Identity;

namespace EmployeeHub.Models.Entities
{
    public class Roles : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}