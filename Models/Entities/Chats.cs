namespace EmployeeHub.Models.Entities;

public class Chat
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }

    public Guid User2Id { get; set; }

    public string? MessagesJson { get; set; } 

    public Guid CreatedBy { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string? History { get; set; }

    public virtual User User1 { get; set; } = null!;

    public virtual User User2 { get; set; } = null!;
}
