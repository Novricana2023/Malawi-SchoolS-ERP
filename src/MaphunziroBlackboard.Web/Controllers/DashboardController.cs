using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Infrastructure.Data;
using MaphunziroBlackboard.Domain.Entities;
using MaphunziroBlackboard.Web.Models;
using System.Security.Claims;

namespace MaphunziroBlackboard.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        var dashboardViewModel = new DashboardViewModel
        {
            UserRoles = userRoles,
            UserName = User.Identity?.Name ?? "",
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? ""
        };

        // Get data based on user roles
        if (userRoles.Contains("SuperAdmin") || userRoles.Contains("SchoolAdmin"))
        {
            await GetAdminDashboardData(dashboardViewModel);
        }
        else if (userRoles.Contains("Teacher"))
        {
            await GetTeacherDashboardData(dashboardViewModel, userId);
        }
        else if (userRoles.Contains("Student"))
        {
            await GetStudentDashboardData(dashboardViewModel, userId);
        }
        else if (userRoles.Contains("Parent"))
        {
            await GetParentDashboardData(dashboardViewModel, userId);
        }

        return View(dashboardViewModel);
    }

    private Task GetAdminDashboardData(DashboardViewModel model)
    {
        model.TotalStudents = 0;
        model.TotalTeachers = 0;
        model.TotalCourses = 0;
        model.TotalClasses = 0;
        model.ActiveStudents = 0;
        model.ActiveTeachers = 0;
        model.TodayAttendance = 0;
        model.PendingAssignments = 0;
        model.UnreadNotifications = 0;
        model.RecentActivities = new List<ActivityItem>();
        model.EnrollmentStats = new List<EnrollmentStat>();
        model.AttendanceStats = new List<AttendanceStat>();
        model.GradeDistribution = new List<GradeDistribution>();
        return Task.CompletedTask;
    }

    private async Task GetTeacherDashboardData(DashboardViewModel model, string userId)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        
        if (teacher != null)
        {
            model.MyCourses = await _context.TeacherCourses
                .Include(tc => tc.Course)
                .Where(tc => tc.TeacherId == teacher.Id && tc.IsActive && !tc.IsDeleted)
                .Select(tc => tc.Course)
                .ToListAsync();
            
            model.MyAssignments = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                .OrderByDescending(a => a.DueDate)
                .Take(10)
                .ToListAsync();
            
            model.PendingSubmissions = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Where(s => s.Assignment.TeacherId == teacher.Id && s.Status == SubmissionStatus.Submitted && !s.IsDeleted)
                .CountAsync();
            
            model.MyClasses = await _context.ClassTeachers
                .Include(ct => ct.Class)
                .Where(ct => ct.TeacherId == teacher.Id && ct.IsActive && !ct.IsDeleted)
                .Select(ct => ct.Class)
                .ToListAsync();
            
            model.TodaySchedule = await GetTeacherSchedule(teacher.Id);
        }
    }

    private async Task GetStudentDashboardData(DashboardViewModel model, string userId)
    {
        var student = await _context.Students
            .Include(s => s.CurrentClass)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        
        if (student != null)
        {
            model.MyCourses = await _context.StudentCourses
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == student.Id && sc.Status == EnrollmentStatus.Active && !sc.IsDeleted)
                .Select(sc => sc.Course)
                .ToListAsync();
            
            // model.MyAssignments = await _context.Assignments
                // .Include(a => a.Course)
                // .Where(a => a.Course.StudentCourses.Any(sc => sc.StudentId == student.Id) && !a.IsDeleted)
                // .OrderByDescending(a => a.DueDate)
                // .Take(10)
                // .Select(a => new AssignmentSummary
                // {
                //     Id = a.Id,
                //     Title = a.Title,
                //     CourseTitle = a.Course.Title,
                //     DueDate = a.DueDate,
                //     Status = a.Submissions.Any(s => s.StudentId == student.Id) ? 
                //         AssignmentStatus.Submitted : AssignmentStatus.Pending
                // })
                // .ToListAsync();
            
            model.StudentAssignments = new List<AssignmentSummary>();
            
            model.StudentPendingAssignments = 0; // model.StudentAssignments.Count(a => a.Status == AssignmentStatus.Pending);
            
            model.MyGrades = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Course)
                .Where(s => s.StudentId == student.Id && s.Score.HasValue && !s.IsDeleted)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(10)
                .Select(s => new GradeSummary
                {
                    AssignmentTitle = s.Assignment.Title,
                    CourseTitle = s.Assignment.Course.Title,
                    Score = s.Score.Value,
                    MaxScore = s.Assignment.MaxScore,
                    SubmittedAt = s.SubmittedAt
                })
                .ToListAsync();
            
            model.MyAttendance = await _context.Attendances
                .Where(a => a.StudentId == student.Id && !a.IsDeleted)
                .OrderByDescending(a => a.Date)
                .Take(10)
                .ToListAsync();
            
            model.AttendancePercentage = await GetStudentAttendancePercentage(student.Id);
            
            model.CurrentGPA = (double)await GetStudentGPA(student.Id);
        }
    }

    private async Task GetParentDashboardData(DashboardViewModel model, string userId)
    {
        var parent = await _context.Parents
            .Include(p => p.Students)
                .ThenInclude(sp => sp.Student)
            .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        
        if (parent != null)
        {
            model.MyChildren = parent.Students
                .Where(sp => !sp.IsDeleted && !sp.Student.IsDeleted)
                .Select(sp => sp.Student)
                .ToList();
            
            var studentIds = model.MyChildren.Select(s => s.Id).ToList();
            
            model.ChildrenAttendance = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => studentIds.Contains(a.StudentId) && a.Date.Date >= DateTime.Today.AddDays(-7) && !a.IsDeleted)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
            
            model.ChildrenGrades = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(s => s.Student)
                .Where(s => studentIds.Contains(s.StudentId) && s.Score.HasValue && !s.IsDeleted)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(20)
                .ToListAsync();
            
            model.UpcomingEvents = await GetUpcomingEventsForParents(studentIds);
        }
    }

    private async Task<List<ScheduleItem>> GetTeacherSchedule(int teacherId)
    {
        return await _context.TimeTables
            .Include(t => t.Class)
            .Include(t => t.SubjectEntity)
            .Where(t => t.TeacherId == teacherId && 
                        t.DayOfWeek == DateTime.Today.DayOfWeek && 
                        !t.IsDeleted)
            .OrderBy(t => t.StartTime)
            .Select(t => new ScheduleItem
            {
                Subject = t.SubjectEntity.Name,
                Class = t.Class.Name,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Room = t.RoomNumber
            })
            .ToListAsync();
    }

    private async Task<double> GetStudentAttendancePercentage(int studentId)
    {
        var total = await _context.Attendances
            .CountAsync(a => a.StudentId == studentId && !a.IsDeleted);
        var present = await _context.Attendances
            .CountAsync(a => a.StudentId == studentId && a.Status == AttendanceStatus.Present && !a.IsDeleted);
        
        return total > 0 ? (present * 100.0 / total) : 0;
    }

    private async Task<double> GetStudentGPA(int studentId)
    {
        var submissions = await _context.AssignmentSubmissions
            .Where(s => s.StudentId == studentId && s.Score.HasValue && !s.IsDeleted)
            .ToListAsync();
        
        if (!submissions.Any()) return 0.0;
        
        return (double)submissions.Average(s => s.Score.Value);
    }

    private async Task<List<EventItem>> GetUpcomingEventsForParents(List<int> studentIds)
    {
        return await _context.Announcements
            .Where(a => a.PublishDate <= DateTime.Now && 
                        a.ExpiryDate >= DateTime.Now && 
                        !a.IsDeleted)
            .OrderBy(a => a.PublishDate)
            .Take(5)
            .Select(a => new EventItem
            {
                Title = a.Title,
                Date = a.PublishDate ?? DateTime.Now,
                Type = a.Type.ToString()
            })
            .ToListAsync();
    }
}
