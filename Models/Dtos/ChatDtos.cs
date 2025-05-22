namespace EmployeeHub.Models.Dtos
{

    public class ChatDto
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public List<ChatHistory>? MessagesJson { get; set; }
    }

    public class ChatHistoryDto
    {
        public Guid ReceiverUserId { get; set; }
        public string Message { get; set; }
    }

    public class ChatHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReceiverUserId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? EditedTimestamp { get; set; }
        public DateTime? DeletedTimestamp { get; set; }
        public string? History { get; set; }
    }
    
}


