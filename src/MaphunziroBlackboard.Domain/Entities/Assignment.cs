namespace MaphunziroBlackboard.Domain.Entities;

public class Assignment : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public AssignmentType Type { get; set; }
    public int MaxScore { get; set; } = 100;
    public DateTime DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public bool AllowLateSubmission { get; set; } = false;
    public decimal LateSubmissionPenaltyPercentage { get; set; } = 10;
    public int MaxAttempts { get; set; } = 1;
    public bool IsPublished { get; set; } = false;
    public bool IsGraded { get; set; } = false;
    public string? AssignmentFile { get; set; }
    public int CourseId { get; set; }
    public int ModuleId { get; set; }
    public int TeacherId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual CourseModule Module { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    public virtual ICollection<AssignmentRubric> Rubrics { get; set; } = new List<AssignmentRubric>();
}

public enum AssignmentType
{
    FileUpload = 1,
    TextSubmission = 2,
    OnlineQuiz = 3,
    Presentation = 4,
    Project = 5,
    Practical = 6,
    Research = 7,
    Essay = 8
}

public class AssignmentSubmission : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? BlobName { get; set; }
    public long FileSize { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public string? TeacherComments { get; set; }
    public DateTime? GradedAt { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public bool IsLate { get; set; } = false;
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public int? GradedByTeacherId { get; set; }

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual Teacher? GradedByTeacher { get; set; }
    public virtual ICollection<SubmissionComment> Comments { get; set; } = new List<SubmissionComment>();
}

public enum SubmissionStatus
{
    Draft = 1,
    Submitted = 2,
    Graded = 3,
    Returned = 4,
    Plagiarized = 5
}

public class AssignmentRubric : BaseEntity
{
    public string Criterion { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxPoints { get; set; }
    public int Order { get; set; }
    public int AssignmentId { get; set; }

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual ICollection<RubricLevel> Levels { get; set; } = new List<RubricLevel>();
}

public class RubricLevel : BaseEntity
{
    public string LevelName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int Order { get; set; }
    public int RubricId { get; set; }

    // Navigation properties
    public virtual AssignmentRubric Rubric { get; set; } = null!;
}

public class SubmissionComment : BaseEntity
{
    public string Comment { get; set; } = string.Empty;
    public CommentType Type { get; set; }
    public int SubmissionId { get; set; }
    public string? UserId { get; set; }
    public int? TeacherId { get; set; }
    public int? StudentId { get; set; }

    // Navigation properties
    public virtual AssignmentSubmission Submission { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual Student? Student { get; set; }
}

public enum CommentType
{
    General = 1,
    Feedback = 2,
    Question = 3,
    Correction = 4,
    Praise = 5
}
