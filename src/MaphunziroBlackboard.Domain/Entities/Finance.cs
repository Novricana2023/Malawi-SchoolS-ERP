namespace MaphunziroBlackboard.Domain.Entities;

public class FeeStructure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public FeeType Type { get; set; }
    public string? AcademicYear { get; set; }
    public int GradeLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DueDate { get; set; }
    public int SchoolId { get; set; }

    // Navigation properties
    public virtual School School { get; set; } = null!;
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public enum FeeType
{
    Tuition = 1,
    Registration = 2,
    Examination = 3,
    Library = 4,
    Laboratory = 5,
    Sports = 6,
    Transport = 7,
    Boarding = 8,
    Uniform = 9,
    Books = 10,
    Other = 11
}

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public decimal BalanceAmount { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public string? Notes { get; set; }
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual FeeStructure FeeStructure { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

public enum InvoiceStatus
{
    Pending = 1,
    PartiallyPaid = 2,
    Paid = 3,
    Overdue = 4,
    Cancelled = 5
}

public class InvoiceItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalAmount { get; set; }
    public int InvoiceId { get; set; }

    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}

public class Payment : BaseEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Successful;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? TransactionReference { get; set; }
    public string? BankName { get; set; }
    public string? ChequeNumber { get; set; }
    public string? Notes { get; set; }
    public int InvoiceId { get; set; }
    public int? ParentId { get; set; }
    public int? StudentId { get; set; }

    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Parent? Parent { get; set; }
    public virtual Student? Student { get; set; }
}

public enum PaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    Cheque = 3,
    MobileMoney = 4,
    CreditCard = 5,
    DebitCard = 6,
    OnlinePayment = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Successful = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}
