using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Application.Interfaces;
using MaphunziroBlackboard.Core.Interfaces;
using MaphunziroBlackboard.Domain.Entities;

namespace MaphunziroBlackboard.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Teacher> _teacherRepository;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IRepository<ApplicationUser> userRepository,
        IRepository<Student> studentRepository,
        IRepository<Teacher> teacherRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetUserByStudentNumberAsync(string studentNumber)
    {
        var student = await _studentRepository.FindAsync(s => s.StudentNumber == studentNumber);
        var studentEntity = student.FirstOrDefault();
        
        if (studentEntity?.UserId != null)
        {
            return await _userManager.FindByIdAsync(studentEntity.UserId);
        }
        
        return null;
    }

    public async Task<ApplicationUser?> GetUserByEmployeeNumberAsync(string employeeNumber)
    {
        var teacher = await _teacherRepository.FindAsync(t => t.EmployeeNumber == employeeNumber);
        var teacherEntity = teacher.FirstOrDefault();
        
        if (teacherEntity?.UserId != null)
        {
            return await _userManager.FindByIdAsync(teacherEntity.UserId);
        }
        
        return null;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return new List<ApplicationUser>();

        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole;
    }

    public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {errors}");
        }

        return user;
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to update user: {errors}");
        }

        return user;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> VerifyEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null;
    }

    public async Task<bool> IsStudentNumberUniqueAsync(string studentNumber)
    {
        var students = await _studentRepository.FindAsync(s => s.StudentNumber == studentNumber);
        return !students.Any();
    }

    public async Task<bool> IsEmployeeNumberUniqueAsync(string employeeNumber)
    {
        var teachers = await _teacherRepository.FindAsync(t => t.EmployeeNumber == employeeNumber);
        return !teachers.Any();
    }
}
