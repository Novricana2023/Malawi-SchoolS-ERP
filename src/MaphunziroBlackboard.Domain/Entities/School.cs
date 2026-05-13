namespace MaphunziroBlackboard.Domain.Entities;

public class School : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? PrincipalName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? EstablishedDate { get; set; }
    public SchoolType SchoolType { get; set; }
    public EducationLevel EducationLevel { get; set; }

    // Navigation properties
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<AcademicYear> AcademicYears { get; set; } = new List<AcademicYear>();
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}

public enum SchoolType
{
    Public = 1,
    Private = 2,
    Charter = 3,
    International = 4
}

public enum EducationLevel
{
    Primary = 1,
    Secondary = 2,
    HighSchool = 3,
    Combined = 4
}
