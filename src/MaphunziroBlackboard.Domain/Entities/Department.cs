namespace MaphunziroBlackboard.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? HeadOfDepartment { get; set; }
    public int SchoolId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
