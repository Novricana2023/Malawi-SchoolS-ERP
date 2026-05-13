namespace MaphunziroBlackboard.Domain.Entities;

public class UserRole : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Permissions { get; set; }
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

public class UserSession : BaseEntity
{
    public string SessionToken { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutTime { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

public class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string UserId { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

public class Grade : BaseEntity
{
    public string GradeName { get; set; } = string.Empty;
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }
    public decimal GradePoint { get; set; }
    public string? Remarks { get; set; }
    public int SchoolId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
}

public class StudentPromotion : BaseEntity
{
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public DateTime PromotionDate { get; set; }
    public string? Remarks { get; set; }
    public PromotionStatus Status { get; set; }
    public int StudentId { get; set; }
    public int AcademicYearId { get; set; }

    // Navigation properties
    public virtual Class FromClass { get; set; } = null!;
    public virtual Class ToClass { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
}

public enum PromotionStatus
{
    Promoted = 1,
    NotPromoted = 2,
    Pending = 3,
    Transferred = 4
}

public class DisciplineRecord : BaseEntity
{
    public string Offense { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DisciplineType Type { get; set; }
    public SeverityLevel Severity { get; set; }
    public DateTime IncidentDate { get; set; }
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public string? ActionTaken { get; set; }
    public string? ReportedBy { get; set; }
    public bool IsResolved { get; set; } = false;
    public DateTime? ResolvedDate { get; set; }
    public string? ResolvedBy { get; set; }
    public int StudentId { get; set; }
    public int? TeacherId { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Teacher? Teacher { get; set; } = null!;
}

public enum DisciplineType
{
    Attendance = 1,
    Behavior = 2,
    Academic = 3,
    Uniform = 4,
    Property = 5,
    Violence = 6,
    Bullying = 7,
    Cheating = 8,
    Other = 9
}

public enum SeverityLevel
{
    Minor = 1,
    Moderate = 2,
    Major = 3,
    Severe = 4
}

public class TimeTable : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? RoomNumber { get; set; }
    public string? Building { get; set; }
    public bool IsActive { get; set; } = true;
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }

    // Navigation properties
    public virtual Class Class { get; set; } = null!;
    public virtual Subject SubjectEntity { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
}

public class TeacherCourse : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int TeacherId { get; set; }
    public int CourseId { get; set; }

    // Navigation properties
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}

public class StudentCourse : BaseEntity
{
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public decimal? CurrentGrade { get; set; }
    public decimal? AttendancePercentage { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2,
    Dropped = 3,
    Suspended = 4,
    Inactive = 5
}

public class CourseResource : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ResourceType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public bool IsPublished { get; set; } = false;
    public int DownloadCount { get; set; } = 0;
    public int CourseId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
}

public enum ResourceType
{
    Document = 1,
    Video = 2,
    Audio = 3,
    Image = 4,
    Link = 5,
    Presentation = 6,
    Spreadsheet = 7,
    Archive = 8,
    Other = 9
}

public class DiscussionForum : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsLocked { get; set; } = false;
    public bool IsPinned { get; set; } = false;
    public int ViewCount { get; set; } = 0;
    public int CourseId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<DiscussionPost> Posts { get; set; } = new List<DiscussionPost>();
}

public class DiscussionPost : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsAnswer { get; set; } = false;
    public int LikeCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public int ParentPostId { get; set; }
    public int ForumId { get; set; }
    public string UserId { get; set; }

    // Navigation properties
    public virtual DiscussionForum Forum { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual DiscussionPost? ParentPost { get; set; }
    public virtual ICollection<DiscussionPost> Replies { get; set; } = new List<DiscussionPost>();
}

public class Quiz : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxAttempts { get; set; } = 1;
    public decimal MaxScore { get; set; } = 100;
    public bool IsPublished { get; set; } = false;
    public bool ShuffleQuestions { get; set; } = false;
    public bool ShowResults { get; set; } = true;
    public int CourseId { get; set; }
    public int ModuleId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual CourseModule Module { get; set; } = null!;
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

public class QuizQuestion : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public int Order { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public int QuizId { get; set; }

    // Navigation properties
    public virtual Quiz Quiz { get; set; } = null!;
    public virtual ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}

public class QuizAttempt : BaseEntity
{
    public decimal Score { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int TimeTakenMinutes { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public int QuizId { get; set; }
    public int StudentId { get; set; }

    // Navigation properties
    public virtual Quiz Quiz { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}

public class QuizAnswer : BaseEntity
{
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public decimal PointsEarned { get; set; }
    public int QuizQuestionId { get; set; }
    public int QuizAttemptId { get; set; }

    // Navigation properties
    public virtual QuizQuestion QuizQuestion { get; set; } = null!;
    public virtual QuizAttempt QuizAttempt { get; set; } = null!;
}
