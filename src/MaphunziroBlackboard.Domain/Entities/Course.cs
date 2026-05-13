namespace MaphunziroBlackboard.Domain.Entities;

public class Course : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public int Credits { get; set; }
    public CourseType Type { get; set; }
    public CourseLevel Level { get; set; }
    public string? Syllabus { get; set; }
    public string? Objectives { get; set; }
    public string? Prerequisites { get; set; }
    public string? LearningOutcomes { get; set; }
    public string? AssessmentMethod { get; set; }
    public string? CourseImage { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPublished { get; set; } = false;
    public int MaxStudents { get; set; } = 50;
    public int CurrentStudents { get; set; } = 0;
    public int SchoolId { get; set; }
    public int? DepartmentId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    public virtual ICollection<CourseResource> Resources { get; set; } = new List<CourseResource>();
    public virtual ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}

public enum CourseType
{
    Core = 1,
    Elective = 2,
    Optional = 3,
    Extracurricular = 4
}

public enum CourseLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Mixed = 4
}

public class CourseModule : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CourseId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<ModuleContent> Contents { get; set; } = new List<ModuleContent>();
}

public class ModuleContent : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContentType Type { get; set; }
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public string? ExternalUrl { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; } = false;
    public int DurationMinutes { get; set; } = 0;
    public int ModuleId { get; set; }

    // Navigation properties
    public virtual CourseModule Module { get; set; } = null!;
    public virtual ICollection<StudentProgress> StudentProgress { get; set; } = new List<StudentProgress>();
}

public enum ContentType
{
    Text = 1,
    Video = 2,
    Audio = 3,
    PDF = 4,
    Document = 5,
    Presentation = 6,
    Image = 7,
    Link = 8,
    Quiz = 9,
    Assignment = 10
}

public class StudentProgress : BaseEntity
{
    public int StudentId { get; set; }
    public int ModuleContentId { get; set; }
    public ProgressStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; } = 0;
    public decimal CompletionPercentage { get; set; } = 0;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual ModuleContent ModuleContent { get; set; } = null!;
}

public enum ProgressStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Skipped = 4
}
