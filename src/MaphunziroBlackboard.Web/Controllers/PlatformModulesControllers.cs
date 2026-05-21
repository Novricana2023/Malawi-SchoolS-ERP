using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaphunziroBlackboard.Web.Models;
using MaphunziroBlackboard.Web.Services;
using MaphunziroBlackboard.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MaphunziroBlackboard.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MaphunziroBlackboard.Web.Controllers;

// Stub modules so sidebar/top-nav URLs resolve; replace with real controllers over time.

[Authorize(Roles = "SuperAdmin,SchoolAdmin,Registrar")]
public class StudentsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Students",
            "Learner records, admissions, and class placement — full roster screens will connect here.",
            "fa-graduation-cap",
            "Central place for student profiles and enrollment",
            "Works alongside Courses, Classes, and Attendance"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin")]
public class TeachersController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Teachers",
            "Staff profiles, subjects taught, and workload — administration workspace.",
            "fa-chalkboard-teacher",
            "Invite faculty accounts and link them to subjects/classes"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin,Registrar")]
public class ParentsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Parents",
            "Guardian contacts and links to learners — parent portal features plug in here.",
            "fa-users",
            "Pair parents with students for messaging and reports"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
public class ClassesController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Classes",
            "Streams, levels, and roll lists — timetables and enrollments connect here.",
            "fa-school",
            "Teachers see classes they support; admins manage structure"));
}

[Authorize]
public class AssignmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;

    public AssignmentsController(ApplicationDbContext context, IBlobService blobService, IConfiguration configuration)
    {
        _context = context;
        _blobService = blobService;
        _configuration = configuration;
    }

    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Assignments",
            "Create tasks, collect submissions, and grade — integration with Courses is next.",
            "fa-tasks",
            "Students submit work here; teachers review in Teaching hub"));

    [Authorize(Roles = "Student")]
    [HttpGet]
    public async Task<IActionResult> Submit(int id)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (assignment == null)
            return NotFound();

        return View(assignment);
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, IFormFile file, string content)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (assignment == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        if (student == null)
            return Forbid();

        string container = _configuration["BlobStorage:Container"] ?? "assignments";
        string blobUrl = null;
        if (file != null && file.Length > 0)
        {
            blobUrl = await _blobService.UploadFileAsync(file, container);
        }

        var submission = new AssignmentSubmission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Title = assignment.Title,
            Content = content,
            FileName = file?.FileName,
            FileSize = file?.Length ?? 0,
            FilePath = blobUrl,
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted
        };

        _context.AssignmentSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }
}

[Authorize]
public class ExamsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Exams",
            "Exam schedules, papers, and results — wired to Grades and Reports.",
            "fa-file-alt"));
}

[Authorize]
public class GradesController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Grades",
            "Marks, GPA-style summaries, and transcripts — feeds Performance analytics later.",
            "fa-chart-line"));
}

[Authorize]
public class AttendanceController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Attendance",
            "Daily registers by class/subject — integrates with Classes and Timetable.",
            "fa-user-check"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin,Accountant")]
public class FinanceController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Finance",
            "Fees, invoices, and receipts — accountant and school admin workspace.",
            "fa-dollar-sign",
            "Role-guarded: Accountant + school leadership"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin,Librarian")]
public class LibraryController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Library",
            "Catalogue, loans, and reservations — librarian operations.",
            "fa-book-open"));
}

[Authorize]
public class CalendarController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Calendar",
            "School-wide events and term dates — FullCalendar UI hooks here.",
            "fa-calendar"));
}

[Authorize]
public class MessagesController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Messages",
            "Internal messaging between staff, students, and parents.",
            "fa-envelope"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin")]
public class ReportsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Reports",
            "Operational and academic exports — connects to Performance when data grows.",
            "fa-chart-bar"));
}

[Authorize(Roles = "SuperAdmin,SchoolAdmin")]
public class SettingsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Settings",
            "Institution profile, academic years, branding, and integrations.",
            "fa-cogs"));
}

[Authorize]
public class ProfileController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Profile",
            "Your account details and preferences — identity profile editor coming next.",
            "fa-user"));
}

[Authorize]
public class NotificationsController : Controller
{
    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Notifications",
            "Alerts from assignments, announcements, and attendance.",
            "fa-bell"));
}
