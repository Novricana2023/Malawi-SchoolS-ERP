namespace MaphunziroBlackboard.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Author { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public string? ISBN { get; set; }
    public string? Edition { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? Category { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public int PageCount { get; set; }
    public string? CoverImage { get; set; }
    public BookType Type { get; set; }
    public decimal Price { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string? Location { get; set; }
    public string? ShelfNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public int SchoolId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual ICollection<BookBorrowing> Borrowings { get; set; } = new List<BookBorrowing>();
    public virtual ICollection<BookReservation> Reservations { get; set; } = new List<BookReservation>();
}

public enum BookType
{
    Textbook = 1,
    Reference = 2,
    Fiction = 3,
    NonFiction = 4,
    Journal = 5,
    Magazine = 6,
    Newspaper = 7,
    Digital = 8,
    Audio = 9,
    Video = 10
}

public class BookBorrowing : BaseEntity
{
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;
    public decimal? LateFee { get; set; }
    public string? Notes { get; set; }
    public int BookId { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public string? ProcessedBy { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
}

public enum BorrowingStatus
{
    Active = 1,
    Returned = 2,
    Overdue = 3,
    Lost = 4,
    Damaged = 5
}

public class BookReservation : BaseEntity
{
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    public string? Notes { get; set; }
    public int BookId { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
}

public enum ReservationStatus
{
    Active = 1,
    Fulfilled = 2,
    Cancelled = 3,
    Expired = 4
}
