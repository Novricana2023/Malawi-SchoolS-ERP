namespace MaphunziroBlackboard.Domain.Entities;

public class Teacher : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
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
    public string? ProfilePicture { get; set; }
    public DateTime EmploymentDate { get; set; }
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;
    public string? Qualifications { get; set; }
    public string? Specialization { get; set; }
    public decimal Salary { get; set; }
    public int SchoolId { get; set; }
    public int? DepartmentId { get; set; }
    public string? UserId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<TeacherCourse> Courses { get; set; } = new List<TeacherCourse>();
    public virtual ICollection<ClassTeacher> AssignedClasses { get; set; } = new List<ClassTeacher>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public string FullName => string.IsNullOrWhiteSpace(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
}

public enum TeacherStatus
{
    Active = 1,
    Inactive = 2,
    OnLeave = 3,
    Terminated = 4,
    Retired = 5
}
