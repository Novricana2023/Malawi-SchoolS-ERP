namespace MaphunziroBlackboard.Domain.Entities;

public class AcademicYear : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int SchoolId { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual ICollection<Term> Terms { get; set; } = new List<Term>();
    public virtual ICollection<StudentPromotion> Promotions { get; set; } = new List<StudentPromotion>();
}

public class Term : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int AcademicYearId { get; set; }
    public int Order { get; set; }

    // Navigation properties
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
