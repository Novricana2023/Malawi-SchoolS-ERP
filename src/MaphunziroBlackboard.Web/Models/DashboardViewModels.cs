using MaphunziroBlackboard.Domain.Entities;

namespace MaphunziroBlackboard.Web.Models;

public class DashboardViewModel
{
    // User Information
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public List<string> UserRoles { get; set; } = new List<string>();

    // Admin Statistics
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalClasses { get; set; }
    public int ActiveStudents { get; set; }
    public int ActiveTeachers { get; set; }
    public int TodayAttendance { get; set; }
    public int PendingAssignments { get; set; }
    public int UnreadNotifications { get; set; }

    // Teacher Data
    public List<Course> MyCourses { get; set; } = new List<Course>();
    public List<Assignment> MyAssignments { get; set; } = new List<Assignment>();
    public int PendingSubmissions { get; set; }
    public List<Class> MyClasses { get; set; } = new List<Class>();
    public List<ScheduleItem> TodaySchedule { get; set; } = new List<ScheduleItem>();

    // Student Data
    public List<AssignmentSummary> StudentAssignments { get; set; } = new List<AssignmentSummary>();
    public int StudentPendingAssignments { get; set; }
    public List<GradeSummary> MyGrades { get; set; } = new List<GradeSummary>();
    public List<Attendance> MyAttendance { get; set; } = new List<Attendance>();
    public double AttendancePercentage { get; set; }
    public double CurrentGPA { get; set; }

    // Parent Data
    public List<Student> MyChildren { get; set; } = new List<Student>();
    public List<Attendance> ChildrenAttendance { get; set; } = new List<Attendance>();
    public List<AssignmentSubmission> ChildrenGrades { get; set; } = new List<AssignmentSubmission>();
    public List<EventItem> UpcomingEvents { get; set; } = new List<EventItem>();

    // Analytics Data
    public List<ActivityItem> RecentActivities { get; set; } = new List<ActivityItem>();
    public List<EnrollmentStat> EnrollmentStats { get; set; } = new List<EnrollmentStat>();
    public List<AttendanceStat> AttendanceStats { get; set; } = new List<AttendanceStat>();
    public List<GradeDistribution> GradeDistribution { get; set; } = new List<GradeDistribution>();
}

// Supporting View Models
public class ActivityItem
{
    public string Action { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class EnrollmentStat
{
    public string ClassName { get; set; } = string.Empty;
    public int Enrolled { get; set; }
    public int Capacity { get; set; }
    public double Percentage => Capacity > 0 ? (Enrolled * 100.0 / Capacity) : 0;
}

public class AttendanceStat
{
    public DateTime Date { get; set; }
    public int Total { get; set; }
    public int Present { get; set; }
    public double Percentage { get; set; }
}

public class GradeDistribution
{
    public string Grade { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AssignmentSummary
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public AssignmentStatus Status { get; set; }
}

public class GradeSummary
{
    public string AssignmentTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime SubmittedAt { get; set; }
    public double Percentage => MaxScore > 0 ? (double)(Score * 100m / MaxScore) : 0;
}

public class ScheduleItem
{
    public string Subject { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}

public class EventItem
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
}

public enum AssignmentStatus
{
    Pending,
    Submitted,
    Graded,
    Overdue
}
