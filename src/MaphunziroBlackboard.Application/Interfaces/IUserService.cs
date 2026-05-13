using MaphunziroBlackboard.Domain.Entities;

namespace MaphunziroBlackboard.Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByStudentNumberAsync(string studentNumber);
    Task<ApplicationUser?> GetUserByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string userId, string newPassword);
    Task<bool> VerifyEmailAsync(string userId);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsStudentNumberUniqueAsync(string studentNumber);
    Task<bool> IsEmployeeNumberUniqueAsync(string employeeNumber);
}
