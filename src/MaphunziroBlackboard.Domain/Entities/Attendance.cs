namespace MaphunziroBlackboard.Domain.Entities;

public class Attendance : BaseEntity
{
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Notes { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TermId { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Term Term { get; set; } = null!;
}

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    Excused = 4,
    Sick = 5,
    Permission = 6
}

public class AttendanceSummary : BaseEntity
{
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int ExcusedCount { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }

    // Navigation properties
    public virtual Class Class { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
}
