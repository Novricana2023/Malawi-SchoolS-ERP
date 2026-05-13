using Microsoft.AspNetCore.Identity;

namespace MaphunziroBlackboard.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? NationalId { get; set; }
    public string? StudentId { get; set; }
    public string? EmployeeId { get; set; }

    // Navigation properties
    public virtual ICollection<Student> StudentProfiles { get; set; } = new List<Student>();
    public virtual ICollection<Teacher> TeacherProfiles { get; set; } = new List<Teacher>();
    public virtual ICollection<Parent> ParentProfiles { get; set; } = new List<Parent>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public string FullName => string.IsNullOrWhiteSpace(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
}
