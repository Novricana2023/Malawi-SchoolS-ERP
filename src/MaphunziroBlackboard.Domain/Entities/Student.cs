namespace MaphunziroBlackboard.Domain.Entities;

public class Student : BaseEntity
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? PassportNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? MedicalInformation { get; set; }
    public string? Allergies { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime AdmissionDate { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public int SchoolId { get; set; }
    public int? CurrentClassId { get; set; }
    public int? CurrentStreamId { get; set; }
    public string? UserId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual Class? CurrentClass { get; set; }
    public virtual Stream? CurrentStream { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<StudentParent> Parents { get; set; } = new List<StudentParent>();
    public virtual ICollection<StudentCourse> Courses { get; set; } = new List<StudentCourse>();
    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual ICollection<DisciplineRecord> DisciplineRecords { get; set; } = new List<DisciplineRecord>();
    public virtual ICollection<StudentPromotion> Promotions { get; set; } = new List<StudentPromotion>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public string FullName => string.IsNullOrWhiteSpace(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
}

public enum StudentStatus
{
    Active = 1,
    Inactive = 2,
    Graduated = 3,
    Transferred = 4,
    Suspended = 5,
    Expelled = 6
}
