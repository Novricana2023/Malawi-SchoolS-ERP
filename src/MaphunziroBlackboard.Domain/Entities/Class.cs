namespace MaphunziroBlackboard.Domain.Entities;

public class Class : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeLevel { get; set; }
    public int Capacity { get; set; } = 40;
    public int CurrentStudents { get; set; } = 0;
    public string? RoomNumber { get; set; }
    public string? Building { get; set; }
    public int SchoolId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual ICollection<Stream> Streams { get; set; } = new List<Stream>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<ClassTeacher> Teachers { get; set; } = new List<ClassTeacher>();
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

public class Stream : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Capacity { get; set; } = 20;
    public int CurrentStudents { get; set; } = 0;
    public int ClassId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Class Class { get; set; } = null!;
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}

public class ClassTeacher : BaseEntity
{
    public int ClassId { get; set; }
    public int TeacherId { get; set; }
    public TeacherRole Role { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Class Class { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
}

public enum TeacherRole
{
    ClassTeacher = 1,
    AssistantTeacher = 2,
    SubjectTeacher = 3,
    ReliefTeacher = 4
}

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Abbreviation { get; set; }
    public SubjectType Type { get; set; }
    public int Credits { get; set; }
    public bool IsCore { get; set; } = false;
    public int ClassId { get; set; }
    public int? DepartmentId { get; set; }

    // Navigation properties
    public virtual Class Class { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual ICollection<TeacherSubject> Teachers { get; set; } = new List<TeacherSubject>();
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
}

public enum SubjectType
{
    Core = 1,
    Elective = 2,
    Optional = 3,
    Extracurricular = 4
}

public class TeacherSubject : BaseEntity
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
}
