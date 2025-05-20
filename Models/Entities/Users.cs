using Microsoft.AspNetCore.Identity;

namespace EmployeeHub.Models.Entities
{
    public partial class User : IdentityUser
    {
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public byte ApprovalStatus { get; set; } = 0;

        public Guid? DeletedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual ICollection<Chat> Chat { get; set; } = new List<Chat>();
    }
}