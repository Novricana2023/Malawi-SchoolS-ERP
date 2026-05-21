using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Application.Interfaces;
using MaphunziroBlackboard.Domain.Entities;
using MaphunziroBlackboard.Infrastructure.Data;
using MaphunziroBlackboard.Web.Models;
using System.Security.Claims;

namespace MaphunziroBlackboard.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserService userService,
        ApplicationDbContext context,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        // Prevent signed-in users from accessing the login page
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        // Prevent caching so the browser back button won't show the login page after sign-in
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        ViewData["ReturnUrl"] = returnUrl;
        await PopulateExternalProvidersAsync();
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        await PopulateExternalProvidersAsync();
        
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToLocal(returnUrl);
            }
            
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                ModelState.AddModelError(string.Empty, "Account has been locked out. Please try again later.");
                return View(model);
            }
            
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Account is not allowed to sign in. Please verify your email.");
                return View(model);
            }
            
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        ViewData["ReturnUrl"] = returnUrl;
        await PopulateExternalProvidersAsync();
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        await PopulateExternalProvidersAsync();
        
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                
                // Add user role based on selection
                await _userManager.AddToRoleAsync(user, model.UserRole);
                await EnsureRoleProfileAsync(user, model.UserRole);
                
                // Send email confirmation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { userId = user.Id, code = code },
                protocol: Request.Scheme);
                
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                return RedirectToLocal(returnUrl);
            }
            
            AddErrors(result);
        }

        return View(model);
    }

    /// <summary>Logout supports GET (navbar link) and POST (forms).</summary>
    [AcceptVerbs("GET", "POST")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        if (_signInManager.IsSignedIn(User))
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
        }

        return View("ConfirmEmail");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword", "Account",
                new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _userService.ResetPasswordAsync(user.Id, "NewSecurePassword123!");
            
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code = null)
    {
        if (code == null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }
        
        var model = new ResetPasswordViewModel { Code = code };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        AddErrors(result);
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return RedirectToAction(nameof(Login));
        }

        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            await PopulateExternalProvidersAsync();
            return View(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
            return RedirectToLocal(returnUrl);
        }
        
        if (result.IsLockedOut)
        {
            return View("Lockout");
        }
        else
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";
            return View("ExternalLogin", new ExternalLoginViewModel
            {
                Email = email ?? "",
                FirstName = firstName,
                LastName = lastName
            });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.UserRole);
                    await EnsureRoleProfileAsync(user, model.UserRole);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            
            AddErrors(result);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(nameof(ExternalLogin), model);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    private async Task PopulateExternalProvidersAsync()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        var providers = schemes.Select(s => s.Name).Where(name => !string.IsNullOrWhiteSpace(name)).ToList();
        ViewData["ExternalProviders"] = providers;
    }

    private async Task EnsureRoleProfileAsync(ApplicationUser user, string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return;
        }

        switch (role.Trim())
        {
            case "Teacher":
                await EnsureTeacherProfileAsync(user);
                break;
            case "Student":
                await EnsureStudentProfileAsync(user);
                break;
            case "Parent":
                await EnsureParentProfileAsync(user);
                break;
        }
    }

    private async Task<int> GetDefaultSchoolIdAsync()
    {
        var schoolId = await _context.Schools
            .OrderBy(s => s.Id)
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        if (schoolId <= 0)
        {
            throw new InvalidOperationException("A school record must exist before creating role profiles.");
        }

        return schoolId;
    }

    private async Task EnsureTeacherProfileAsync(ApplicationUser user)
    {
        if (await _context.Teachers.AnyAsync(t => t.UserId == user.Id && !t.IsDeleted))
        {
            return;
        }

        var employeeNumber = await GenerateUniqueTeacherNumberAsync();
        var schoolId = await GetDefaultSchoolIdAsync();

        var teacher = new Teacher
        {
            UserId = user.Id,
            EmployeeNumber = employeeNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth ?? DateTime.UtcNow.AddYears(-25),
            Gender = user.Gender ?? "Not specified",
            Email = user.Email,
            Phone = user.PhoneNumber,
            EmploymentDate = DateTime.UtcNow,
            Salary = 0,
            SchoolId = schoolId,
            Status = TeacherStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
    }

    private async Task EnsureStudentProfileAsync(ApplicationUser user)
    {
        if (await _context.Students.AnyAsync(s => s.UserId == user.Id && !s.IsDeleted))
        {
            return;
        }

        var studentNumber = await GenerateUniqueStudentNumberAsync();
        var schoolId = await GetDefaultSchoolIdAsync();

        var student = new Student
        {
            UserId = user.Id,
            StudentNumber = studentNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth ?? DateTime.UtcNow.AddYears(-16),
            Gender = user.Gender ?? "Not specified",
            Email = user.Email,
            Phone = user.PhoneNumber,
            AdmissionDate = DateTime.UtcNow,
            SchoolId = schoolId,
            Status = StudentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    private async Task EnsureParentProfileAsync(ApplicationUser user)
    {
        if (await _context.Parents.AnyAsync(p => p.UserId == user.Id && !p.IsDeleted))
        {
            return;
        }

        var parent = new Parent
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth ?? DateTime.UtcNow.AddYears(-35),
            Gender = user.Gender ?? "Not specified",
            Email = user.Email,
            Phone = user.PhoneNumber,
            Status = ParentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Parents.Add(parent);
        await _context.SaveChangesAsync();
    }

    private async Task<string> GenerateUniqueStudentNumberAsync()
    {
        while (true)
        {
            var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            var candidate = $"STU-{DateTime.UtcNow:yy}-{suffix}";
            var exists = await _context.Students.AnyAsync(s => s.StudentNumber == candidate);
            if (!exists)
            {
                return candidate;
            }
        }
    }

    private async Task<string> GenerateUniqueTeacherNumberAsync()
    {
        while (true)
        {
            var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            var candidate = $"EMP-{DateTime.UtcNow:yy}-{suffix}";
            var exists = await _context.Teachers.AnyAsync(t => t.EmployeeNumber == candidate);
            if (!exists)
            {
                return candidate;
            }
        }
    }
}
