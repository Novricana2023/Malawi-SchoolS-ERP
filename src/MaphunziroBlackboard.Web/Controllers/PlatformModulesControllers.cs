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
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(ApplicationDbContext context, IBlobService blobService, IConfiguration configuration, ILogger<AssignmentsController> logger)
    {
        _context = context;
        _blobService = blobService;
        _configuration = configuration;
        _logger = logger;
    }

    public IActionResult Index() =>
        View("ModuleWorkspace", ModuleWorkspaceViewModel.For(
            "Assignments",
            "Create tasks, collect submissions, and grade — integration with Courses is next.",
            "fa-tasks",
            "Students submit work here; teachers review in Teaching hub"));

    [Authorize(Roles = "Teacher")]
    [HttpGet]
    public async Task<IActionResult> ListSubmissions(int assignmentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        if (teacher == null) return Forbid();

        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId && !a.IsDeleted);
        if (assignment == null) return NotFound();
        if (assignment.TeacherId != teacher.Id) return Forbid();

        var submissions = await _context.AssignmentSubmissions
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId && !s.IsDeleted)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

        return View("ListSubmissions", submissions);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet]
    public async Task<IActionResult> Grade(int id)
    {
        var submission = await _context.AssignmentSubmissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (submission == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        if (teacher == null) return Forbid();
        if (submission.Assignment.TeacherId != teacher.Id) return Forbid();

        return View(submission);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Grade(int id, decimal? score, string feedback)
    {
        var submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (submission == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        if (teacher == null) return Forbid();

        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == submission.AssignmentId && !a.IsDeleted);
        if (assignment == null || assignment.TeacherId != teacher.Id) return Forbid();

        submission.Score = score;
        submission.Feedback = feedback;
        submission.Status = SubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Teacher {TeacherId} graded submission {SubmissionId} with score {Score}", teacher.Id, submission.Id, score);
        return RedirectToAction("ListSubmissions", new { assignmentId = submission.AssignmentId });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> DownloadSubmission(int id)
    {
        var submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (submission == null) return NotFound();

        if (!string.IsNullOrEmpty(submission.BlobName))
        {
            var container = _configuration["BlobStorage:Container"] ?? "assignments";
            // Try generate SAS via service; if not available, redirect to stored FilePath
            var sasUrl = (_blobService as AzureBlobService)?.GenerateBlobSasUrl(container, submission.BlobName, 60);
            if (!string.IsNullOrEmpty(sasUrl)) return Redirect(sasUrl);
            if (!string.IsNullOrEmpty(submission.FilePath)) return Redirect(submission.FilePath);
        }

        return NotFound();
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        var submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (submission == null) return NotFound();

        if (!string.IsNullOrEmpty(submission.BlobName))
        {
            var container = _configuration["BlobStorage:Container"] ?? "assignments";
            await _blobService.DeleteFileAsync(container, submission.BlobName);
        }

        submission.IsDeleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction("ListSubmissions", new { assignmentId = submission.AssignmentId });
    }

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
        BlobUploadResult? uploadResult = null;

        // Basic file validation
        var maxSize = long.TryParse(_configuration["FileValidation:MaxBytes"], out var m) ? m : 10 * 1024 * 1024;
        var allowed = (_configuration["FileValidation:AllowedTypes"] ?? "pdf,doc,docx,txt,zip").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (file != null && file.Length > 0)
        {
            if (file.Length > maxSize)
                ModelState.AddModelError("file", $"File size exceeds limit of {maxSize} bytes.");

            var ext = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
            if (!allowed.Contains(ext))
                ModelState.AddModelError("file", "File type not allowed.");

            if (!ModelState.IsValid)
            {
                return View(assignment);
            }

            uploadResult = await _blobService.UploadFileAsync(file, container);
        }

        var submission = new AssignmentSubmission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Title = assignment.Title,
            Content = content,
            FileName = file?.FileName,
            FileSize = file?.Length ?? 0,
            FilePath = uploadResult?.Url,
            BlobName = uploadResult?.BlobName,
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
