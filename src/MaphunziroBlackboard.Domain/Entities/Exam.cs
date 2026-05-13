namespace MaphunziroBlackboard.Domain.Entities;

public class Exam : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public ExamType Type { get; set; }
    public DateTime ExamDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxScore { get; set; } = 100;
    public int PassScore { get; set; } = 50;
    public bool IsPublished { get; set; } = false;
    public bool AllowRetake { get; set; } = false;
    public int MaxAttempts { get; set; } = 1;
    public int CourseId { get; set; }
    public int TermId { get; set; }
    public int TeacherId { get; set; }
    public int ClassId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual Term Term { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
    public virtual ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
    public virtual ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
}

public enum ExamType
{
    Quiz = 1,
    Test = 2,
    Midterm = 3,
    Final = 4,
    Practical = 5,
    Assignment = 6,
    Project = 7
}

public class ExamResult : BaseEntity
{
    public decimal Score { get; set; }
    public decimal Percentage { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    public int AttemptNumber { get; set; } = 1;
    public int TimeTakenMinutes { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    // Navigation properties
    public virtual Exam Exam { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<AnsweredQuestion> AnsweredQuestions { get; set; } = new List<AnsweredQuestion>();
}

public class ExamQuestion : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public string? QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? CorrectAnswer { get; set; }
    public string? Options { get; set; }
    public string? Explanation { get; set; }
    public int ExamId { get; set; }

    // Navigation properties
    public virtual Exam Exam { get; set; } = null!;
    public virtual ICollection<AnsweredQuestion> Answers { get; set; } = new List<AnsweredQuestion>();
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4,
    FillBlank = 5,
    Matching = 6,
    FileUpload = 7
}

public class AnsweredQuestion : BaseEntity
{
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public decimal PointsEarned { get; set; }
    public string? Explanation { get; set; }
    public int ExamQuestionId { get; set; }
    public int ExamResultId { get; set; }

    // Navigation properties
    public virtual ExamQuestion ExamQuestion { get; set; } = null!;
    public virtual ExamResult ExamResult { get; set; } = null!;
}
