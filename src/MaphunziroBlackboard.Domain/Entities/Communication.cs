namespace MaphunziroBlackboard.Domain.Entities;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public AnnouncementType Type { get; set; }
    public Priority Priority { get; set; } = Priority.Normal;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool SendEmail { get; set; } = false;
    public bool SendSMS { get; set; } = false;
    public string? PostedBy { get; set; }
    public int SchoolId { get; set; }
    public int? CourseId { get; set; }
    public int? ClassId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual Course? Course { get; set; }
    public virtual Class? Class { get; set; }
    public virtual ICollection<AnnouncementRecipient> Recipients { get; set; } = new List<AnnouncementRecipient>();
}

public enum AnnouncementType
{
    General = 1,
    Academic = 2,
    Event = 3,
    Emergency = 4,
    Holiday = 5,
    Examination = 6,
    Result = 7,
    Fee = 8,
    Maintenance = 9
}

public enum Priority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4
}

public class AnnouncementRecipient : BaseEntity
{
    public int AnnouncementId { get; set; }
    public string? UserId { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public int? ParentId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public virtual Announcement Announcement { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
    public virtual Student? Student { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual Parent? Parent { get; set; }
}

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Priority Priority { get; set; } = Priority.Normal;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? UserId { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public int? ParentId { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual Student? Student { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual Parent? Parent { get; set; }
}

public enum NotificationType
{
    Assignment = 1,
    Grade = 2,
    Announcement = 3,
    Attendance = 4,
    Fee = 5,
    Exam = 6,
    Message = 7,
    System = 8,
    Reminder = 9,
    Alert = 10
}

public class Message : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public string? ReplyTo { get; set; }
    public int ParentMessageId { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }

    // Navigation properties
    public virtual ApplicationUser Sender { get; set; } = null!;
    public virtual ApplicationUser Receiver { get; set; } = null!;
    public virtual Message? ParentMessage { get; set; }
    public virtual ICollection<Message> Replies { get; set; } = new List<Message>();
    public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}

public enum MessageType
{
    Text = 1,
    Email = 2,
    SMS = 3,
    System = 4
}

public enum MessageStatus
{
    Draft = 1,
    Sent = 2,
    Delivered = 3,
    Read = 4,
    Failed = 5,
    Deleted = 6
}

public class MessageAttachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int MessageId { get; set; }

    // Navigation properties
    public virtual Message Message { get; set; } = null!;
}
