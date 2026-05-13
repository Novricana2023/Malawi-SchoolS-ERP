using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Infrastructure.Data;
using MaphunziroBlackboard.Domain.Entities;
using System.Security.Claims;

namespace MaphunziroBlackboard.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ApplicationDbContext context, ILogger<CoursesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Courses
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        IQueryable<Course> coursesQuery = _context.Courses
            .Include(c => c.School)
            .Include(c => c.Department)
            // .Include(c => c.Teachers)
            //     .ThenInclude(tc => tc.Teacher)
            .Where(c => !c.IsDeleted);

        // Filter based on user role
        if (userRoles.Contains("Student"))
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
            
            if (student != null)
            {
                // coursesQuery = coursesQuery.Where(c => 
                    // c.StudentCourses.Any(sc => sc.StudentId == student.Id && !sc.IsDeleted));
            }
        }
        else if (userRoles.Contains("Teacher"))
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                    .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
            
            if (teacher != null)
            {
                coursesQuery = coursesQuery.Where(c => 
                    c.TeacherCourses.Any(tc => tc.TeacherId == teacher.Id && !tc.IsDeleted));
            }
        }

        var courses = await coursesQuery
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(courses);
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.School)
            .Include(c => c.Department)
            // .Include(c => c.Teachers)
                // .ThenInclude(tc => tc.Teacher)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Contents)
            .Include(c => c.Assignments)
            .Include(c => c.Quizzes)
            .Include(c => c.Announcements)
            .Include(c => c.Resources)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (course == null)
        {
            return NotFound();
        }

        // Check if user has access to this course
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        var hasAccess = false;
        
        if (userRoles.Contains("SuperAdmin") || userRoles.Contains("SchoolAdmin"))
        {
            hasAccess = true;
        }
        else if (userRoles.Contains("Teacher"))
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
            hasAccess = teacher != null && course.TeacherCourses.Any(tc => tc.TeacherId == teacher.Id && !tc.IsDeleted);
        }
        else if (userRoles.Contains("Student"))
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
            hasAccess = student != null && course.StudentCourses.Any(sc => sc.StudentId == student.Id && !sc.IsDeleted);
        }

        if (!hasAccess)
        {
            return Forbid();
        }

        return View(course);
    }

    // GET: Courses/Create
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public IActionResult Create()
    {
        ViewBag.Departments = _context.Departments
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.Name })
            .ToList();
        
        return View();
    }

    // POST: Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            
            // Set school based on user
            if (userRoles.Contains("SuperAdmin"))
            {
                var firstSchool = await _context.Schools.FirstOrDefaultAsync(s => !s.IsDeleted);
                if (firstSchool != null)
                {
                    course.SchoolId = firstSchool.Id;
                }
            }
            else
            {
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
                if (teacher != null)
                {
                    course.SchoolId = teacher.SchoolId;
                }
            }

            course.CreatedBy = userId;
            course.UpdatedBy = userId;

            _context.Add(course);
            await _context.SaveChangesAsync();

            // If teacher is creating the course, assign them to it
            if (userRoles.Contains("Teacher"))
            {
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
                if (teacher != null)
                {
                    var teacherCourse = new TeacherCourse
                    {
                        TeacherId = teacher.Id,
                        CourseId = course.Id,
                        StartDate = DateTime.Now,
                        IsActive = true,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    _context.Add(teacherCourse);
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("User {UserId} created course {CourseId}", userId, course.Id);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Departments = _context.Departments
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.Name })
            .ToList();
        
        return View(course);
    }

    // GET: Courses/Edit/5
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (course == null)
        {
            return NotFound();
        }

        // Check if user has permission to edit this course
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        var hasPermission = false;
        
        if (userRoles.Contains("SuperAdmin") || userRoles.Contains("SchoolAdmin"))
        {
            hasPermission = true;
        }
        else if (userRoles.Contains("Teacher"))
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
            hasPermission = teacher != null && course.TeacherCourses.Any(tc => tc.TeacherId == teacher.Id && !tc.IsDeleted);
        }

        if (!hasPermission)
        {
            return Forbid();
        }

        ViewBag.Departments = _context.Departments
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.Name })
            .ToList();
        
        return View(course);
    }

    // POST: Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                course.UpdatedBy = userId;
                course.UpdatedAt = DateTime.UtcNow;

                _context.Update(course);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} updated course {CourseId}", userId, course.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Departments = _context.Departments
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.Name })
            .ToList();
        
        return View(course);
    }

    // GET: Courses/Delete/5
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses
            .Include(c => c.School)
            .Include(c => c.Department)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (course == null)
        {
            return NotFound();
        }

        // Check if user has permission to delete this course
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        var hasPermission = false;
        
        if (userRoles.Contains("SuperAdmin") || userRoles.Contains("SchoolAdmin"))
        {
            hasPermission = true;
        }
        else if (userRoles.Contains("Teacher"))
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
            hasPermission = teacher != null && course.TeacherCourses.Any(tc => tc.TeacherId == teacher.Id && !tc.IsDeleted);
        }

        if (!hasPermission)
        {
            return Forbid();
        }

        return View(course);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,Teacher")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (course == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        course.IsDeleted = true;
        course.DeletedAt = DateTime.UtcNow;
        course.DeletedBy = userId;

        _context.Update(course);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} deleted course {CourseId}", userId, course.Id);
        
        return RedirectToAction(nameof(Index));
    }

    // POST: Courses/Enroll/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted && c.IsPublished);

        if (course == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);

        if (student == null)
        {
            return Forbid();
        }

        // Check if already enrolled
        var existingEnrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CourseId == course.Id && !sc.IsDeleted);

        if (existingEnrollment != null)
        {
            TempData["Warning"] = "You are already enrolled in this course.";
            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        // Check if course is full
        if (course.CurrentStudents >= course.MaxStudents)
        {
            TempData["Error"] = "This course is already full.";
            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        // Create enrollment
        var studentCourse = new StudentCourse
        {
            StudentId = student.Id,
            CourseId = course.Id,
            EnrollmentDate = DateTime.Now,
            Status = EnrollmentStatus.Active,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        _context.Add(studentCourse);
        
        // Update course student count
        course.CurrentStudents++;
        course.UpdatedBy = userId;
        course.UpdatedAt = DateTime.UtcNow;
        _context.Update(course);

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", student.Id, course.Id);
        
        TempData["Success"] = "Successfully enrolled in the course!";
        return RedirectToAction(nameof(Details), new { id = course.Id });
    }

    // POST: Courses/Unenroll/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Unenroll(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (course == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);

        if (student == null)
        {
            return Forbid();
        }

        var enrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CourseId == course.Id && !sc.IsDeleted);

        if (enrollment == null)
        {
            TempData["Warning"] = "You are not enrolled in this course.";
            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        // Mark enrollment as deleted
        enrollment.IsDeleted = true;
        enrollment.DeletedAt = DateTime.UtcNow;
        enrollment.DeletedBy = userId;
        enrollment.Status = EnrollmentStatus.Inactive;

        _context.Update(enrollment);
        
        // Update course student count
        course.CurrentStudents--;
        course.UpdatedBy = userId;
        course.UpdatedAt = DateTime.UtcNow;
        _context.Update(course);

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Student {StudentId} unenrolled from course {CourseId}", student.Id, course.Id);
        
        TempData["Success"] = "Successfully unenrolled from the course.";
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id && !e.IsDeleted);
    }
}
