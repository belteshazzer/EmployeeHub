using Microsoft.AspNetCore.Identity;

namespace EmployeeHub.Models.Entities
{
    public partial class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        public string? Department { get; set; }
        public string? ProfilePicture { get; set; }

        public byte ApprovalStatus { get; set; } = 0;
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? DeletedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual ICollection<Chat> ChatsAsUser1 { get; set; } = [];
        public virtual ICollection<Chat> ChatsAsUser2 { get; set; } = [];

    }
}