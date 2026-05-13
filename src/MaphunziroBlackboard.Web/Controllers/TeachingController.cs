using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Domain.Entities;
using MaphunziroBlackboard.Infrastructure.Data;
using MaphunziroBlackboard.Web.Models;

namespace MaphunziroBlackboard.Web.Controllers;

/// <summary>
/// Dedicated faculty / lecturer workspace (courses, schedule, assignments).
/// </summary>
[Authorize(Roles = "Teacher")]
public class TeachingController : Controller
{
    private readonly ApplicationDbContext _context;

    public TeachingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var model = new DashboardViewModel
        {
            UserName = User.Identity?.Name ?? "",
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? "",
            UserRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList()
        };

        await LoadTeacherWorkspaceAsync(model, userId);
        return View(model);
    }

    private async Task LoadTeacherWorkspaceAsync(DashboardViewModel model, string userId)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);

        if (teacher == null)
            return;

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
            .Where(s => s.Assignment.TeacherId == teacher.Id && s.Status == SubmissionStatus.Submitted && !s.IsDeleted)
            .CountAsync();

        model.MyClasses = await _context.ClassTeachers
            .Include(ct => ct.Class)
            .Where(ct => ct.TeacherId == teacher.Id && ct.IsActive && !ct.IsDeleted)
            .Select(ct => ct.Class)
            .ToListAsync();

        model.TodaySchedule = await _context.TimeTables
            .Include(t => t.Class)
            .Include(t => t.SubjectEntity)
            .Where(t => t.TeacherId == teacher.Id &&
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
}
