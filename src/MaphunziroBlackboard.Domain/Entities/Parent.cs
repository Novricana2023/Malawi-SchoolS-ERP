namespace MaphunziroBlackboard.Domain.Entities;

public class Parent : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? PassportNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Occupation { get; set; }
    public string? WorkPlace { get; set; }
    public string? WorkPhone { get; set; }
    public string? ProfilePicture { get; set; }
    public ParentStatus Status { get; set; } = ParentStatus.Active;
    public string? UserId { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<StudentParent> Students { get; set; } = new List<StudentParent>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public string FullName => string.IsNullOrWhiteSpace(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";
}

public enum ParentStatus
{
    Active = 1,
    Inactive = 2,
    Deceased = 3
}

public class StudentParent : BaseEntity
{
    public int StudentId { get; set; }
    public int ParentId { get; set; }
    public ParentType ParentType { get; set; }
    public bool IsPrimaryContact { get; set; } = false;
    public bool CanAccessGrades { get; set; } = true;
    public bool CanAccessAttendance { get; set; } = true;
    public bool CanAccessFees { get; set; } = true;
    public bool ReceivesNotifications { get; set; } = true;

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Parent Parent { get; set; } = null!;
}

public enum ParentType
{
    Father = 1,
    Mother = 2,
    Guardian = 3,
    Uncle = 4,
    Aunt = 5,
    Grandparent = 6,
    Sibling = 7,
    Other = 8
}
