using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MaphunziroBlackboard.Domain.Entities;

namespace MaphunziroBlackboard.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // School Management
    public DbSet<School> Schools { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Term> Terms { get; set; }

    // User Management
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<StudentParent> StudentParents { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    // Class Management
    public DbSet<Class> Classes { get; set; }
    public DbSet<MaphunziroBlackboard.Domain.Entities.Stream> Streams { get; set; }
    public DbSet<ClassTeacher> ClassTeachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<TimeTable> TimeTables { get; set; }

    // Learning Management System
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseModule> CourseModules { get; set; }
    public DbSet<ModuleContent> ModuleContents { get; set; }
    public DbSet<StudentProgress> StudentProgress { get; set; }
    public DbSet<TeacherCourse> TeacherCourses { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }
    public DbSet<CourseResource> CourseResources { get; set; }

    // Assignments
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    public DbSet<AssignmentRubric> AssignmentRubrics { get; set; }
    public DbSet<RubricLevel> RubricLevels { get; set; }
    public DbSet<SubmissionComment> SubmissionComments { get; set; }

    // Quizzes
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<QuizAnswer> QuizAnswers { get; set; }

    // Examinations
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    public DbSet<ExamQuestion> ExamQuestions { get; set; }
    public DbSet<AnsweredQuestion> AnsweredQuestions { get; set; }

    // Grading
    public DbSet<Grade> Grades { get; set; }
    public DbSet<StudentPromotion> StudentPromotions { get; set; }

    // Attendance
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<AttendanceSummary> AttendanceSummaries { get; set; }

    // Communication
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<AnnouncementRecipient> AnnouncementRecipients { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }

    // Discussion Forums
    public DbSet<DiscussionForum> DiscussionForums { get; set; }
    public DbSet<DiscussionPost> DiscussionPosts { get; set; }

    // Library
    public DbSet<Book> Books { get; set; }
    public DbSet<BookBorrowing> BookBorrowings { get; set; }
    public DbSet<BookReservation> BookReservations { get; set; }

    // Finance
    public DbSet<FeeStructure> FeeStructures { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    // Discipline
    public DbSet<DisciplineRecord> DisciplineRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(256);
            // entity.Property(u => u.StudentNumber).HasMaxLength(50);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfilePicture).HasMaxLength(500);
        });

        // Configure School
        builder.Entity<School>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Website).HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure Student
        builder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StudentNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.HasOne(e => e.School).WithMany(s => s.Students).HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.CurrentClass).WithMany(c => c.Students).HasForeignKey(e => e.CurrentClassId);
            entity.HasOne(e => e.CurrentStream).WithMany(s => s.Students).HasForeignKey(e => e.CurrentStreamId);
        });

        // Configure Teacher
        builder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasOne(e => e.School).WithMany(s => s.Teachers).HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.Department).WithMany(d => d.Teachers).HasForeignKey(e => e.DepartmentId);
        });

        // Configure Course
        builder.Entity<Course>(entity =>
        {
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasOne(e => e.School).WithMany(s => s.Courses).HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.Department).WithMany(d => d.Courses).HasForeignKey(e => e.DepartmentId);
        });

        // Configure Assignment
        builder.Entity<Assignment>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasOne(e => e.Course).WithMany(c => c.Assignments).HasForeignKey(e => e.CourseId);
            entity.HasOne(e => e.Module).WithMany().HasForeignKey(e => e.ModuleId);
            entity.HasOne(e => e.Teacher).WithMany(t => t.Assignments).HasForeignKey(e => e.TeacherId);
        });

        // Configure Exam
        builder.Entity<Exam>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            // entity.HasOne(e => e.Course).WithMany(c => c.Exams).HasForeignKey(e => e.CourseId);
            entity.HasOne(e => e.Term).WithMany(t => t.Exams).HasForeignKey(e => e.TermId);
            entity.HasOne(e => e.Teacher).WithMany(t => t.Exams).HasForeignKey(e => e.TeacherId);
            // entity.HasOne(e => e.Class).WithMany(c => c.Exams).HasForeignKey(e => e.ClassId);
        });

        // Configure Attendance
        builder.Entity<Attendance>(entity =>
        {
            entity.HasOne(e => e.Student).WithMany(s => s.Attendances).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany(t => t.Attendances).HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Class).WithMany(c => c.Attendances).HasForeignKey(e => e.ClassId);
            // entity.HasOne(e => e.Subject).WithMany(s => s.Attendances).HasForeignKey(e => e.SubjectId);
            entity.HasOne(e => e.Term).WithMany(t => t.Attendances).HasForeignKey(e => e.TermId);
        });

        // Configure Book
        builder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.HasOne(e => e.School).WithMany().HasForeignKey(e => e.SchoolId);
        });

        // Configure Invoice
        builder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            // entity.HasOne(e => e.Student).WithMany(s => s.Invoices).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.FeeStructure).WithMany(f => f.Invoices).HasForeignKey(e => e.FeeStructureId);
        });

        // Configure Payment
        builder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Invoice).WithMany(i => i.Payments).HasForeignKey(e => e.InvoiceId);
            entity.HasOne(e => e.Parent).WithMany(p => p.Payments).HasForeignKey(e => e.ParentId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
        });

        // Configure Announcement
        builder.Entity<Announcement>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.School).WithMany().HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.Course).WithMany(c => c.Announcements).HasForeignKey(e => e.CourseId);
            entity.HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId);
        });

        // Configure Message (Restrict deletes: SQL Server disallows multiple cascade paths from AspNetUsers)
        builder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.Sender).WithMany().HasForeignKey(e => e.SenderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Receiver).WithMany().HasForeignKey(e => e.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ParentMessage).WithMany(m => m.Replies).HasForeignKey(e => e.ParentMessageId).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Notification
        builder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired();
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Parent).WithMany().HasForeignKey(e => e.ParentId);
        });

        // Configure DiscussionForum
        builder.Entity<DiscussionForum>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            // entity.HasOne(e => e.Course).WithMany(c => c.DiscussionForums).HasForeignKey(e => e.CourseId);
        });

        // Configure DiscussionPost
        builder.Entity<DiscussionPost>(entity =>
        {
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.Forum).WithMany(f => f.Posts).HasForeignKey(e => e.ForumId);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.ParentPost).WithMany(p => p.Replies).HasForeignKey(e => e.ParentPostId);
        });

        // Configure Quiz
        builder.Entity<Quiz>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Course).WithMany(c => c.Quizzes).HasForeignKey(e => e.CourseId);
            entity.HasOne(e => e.Module).WithMany().HasForeignKey(e => e.ModuleId);
        });

        // Configure QuizQuestion
        builder.Entity<QuizQuestion>(entity =>
        {
            entity.Property(e => e.Question).IsRequired();
            entity.HasOne(e => e.Quiz).WithMany(q => q.Questions).HasForeignKey(e => e.QuizId);
        });

        // Configure QuizAttempt
        builder.Entity<QuizAttempt>(entity =>
        {
            entity.HasOne(e => e.Quiz).WithMany(q => q.Attempts).HasForeignKey(e => e.QuizId);
            // entity.HasOne(e => e.Student).WithMany(s => s.QuizAttempts).HasForeignKey(e => e.StudentId);
        });

        // Configure QuizAnswer
        builder.Entity<QuizAnswer>(entity =>
        {
            entity.Property(e => e.Answer).IsRequired();
            entity.HasOne(e => e.QuizQuestion).WithMany(q => q.Answers).HasForeignKey(e => e.QuizQuestionId);
            entity.HasOne(e => e.QuizAttempt).WithMany(a => a.Answers).HasForeignKey(e => e.QuizAttemptId);
        });

        // Configure StudentParent
        builder.Entity<StudentParent>(entity =>
        {
            entity.HasOne(e => e.Student).WithMany(s => s.Parents).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Parent).WithMany(p => p.Students).HasForeignKey(e => e.ParentId);
        });

        // Configure ClassTeacher
        builder.Entity<ClassTeacher>(entity =>
        {
            entity.HasOne(e => e.Class).WithMany(c => c.Teachers).HasForeignKey(e => e.ClassId);
            entity.HasOne(e => e.Teacher).WithMany(t => t.AssignedClasses).HasForeignKey(e => e.TeacherId);
        });

        // Configure TeacherSubject
        builder.Entity<TeacherSubject>(entity =>
        {
            // entity.HasOne(e => e.Teacher).WithMany(t => t.Subjects).HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Subject).WithMany(s => s.Teachers).HasForeignKey(e => e.SubjectId);
        });

        // Configure TimeTable
        builder.Entity<TimeTable>(entity =>
        {
            entity.HasOne(e => e.Class).WithMany(c => c.TimeTables).HasForeignKey(e => e.ClassId);
            entity.HasOne(e => e.SubjectEntity).WithMany(s => s.TimeTables).HasForeignKey(e => e.SubjectId);
            // entity.HasOne(e => e.Teacher).WithMany(t => t.TimeTables).HasForeignKey(e => e.TeacherId);
        });

        // Configure StudentPromotion
        builder.Entity<StudentPromotion>(entity =>
        {
            entity.HasOne(e => e.Student).WithMany(s => s.Promotions).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.FromClass).WithMany().HasForeignKey(e => e.FromClassId);
            entity.HasOne(e => e.ToClass).WithMany().HasForeignKey(e => e.ToClassId);
            entity.HasOne(e => e.AcademicYear).WithMany(a => a.Promotions).HasForeignKey(e => e.AcademicYearId);
        });

        // Configure DisciplineRecord
        builder.Entity<DisciplineRecord>(entity =>
        {
            entity.Property(e => e.Offense).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Student).WithMany(s => s.DisciplineRecords).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
        });

        // Configure Grade
        builder.Entity<Grade>(entity =>
        {
            entity.Property(e => e.GradeName).IsRequired().HasMaxLength(10);
            entity.HasOne(e => e.School).WithMany().HasForeignKey(e => e.SchoolId);
        });

        // Configure UserSession
        builder.Entity<UserSession>(entity =>
        {
            entity.Property(e => e.SessionToken).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.User).WithMany(u => u.UserSessions).HasForeignKey(e => e.UserId);
        });

        // Configure AuditLog
        builder.Entity<AuditLog>(entity =>
        {
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.User).WithMany(u => u.AuditLogs).HasForeignKey(e => e.UserId);
        });

        // Configure UserRole
        builder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId);
        });

        // Configure BookBorrowing
        builder.Entity<BookBorrowing>(entity =>
        {
            entity.HasOne(e => e.Book).WithMany(b => b.Borrowings).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
        });

        // Configure BookReservation
        builder.Entity<BookReservation>(entity =>
        {
            entity.HasOne(e => e.Book).WithMany(b => b.Reservations).HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
        });

        // Configure FeeStructure
        builder.Entity<FeeStructure>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.School).WithMany().HasForeignKey(e => e.SchoolId);
        });

        // Configure InvoiceItem
        builder.Entity<InvoiceItem>(entity =>
        {
            entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Invoice).WithMany(i => i.Items).HasForeignKey(e => e.InvoiceId);
        });

        // Configure AnnouncementRecipient
        builder.Entity<AnnouncementRecipient>(entity =>
        {
            entity.HasOne(e => e.Announcement).WithMany(a => a.Recipients).HasForeignKey(e => e.AnnouncementId);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Parent).WithMany().HasForeignKey(e => e.ParentId);
        });

        // Configure MessageAttachment
        builder.Entity<MessageAttachment>(entity =>
        {
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Message).WithMany(m => m.Attachments).HasForeignKey(e => e.MessageId);
        });

        // Configure CourseModule
        builder.Entity<CourseModule>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Course).WithMany(c => c.Modules).HasForeignKey(e => e.CourseId);
        });

        // Configure ModuleContent
        builder.Entity<ModuleContent>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Module).WithMany(m => m.Contents).HasForeignKey(e => e.ModuleId);
        });

        // Configure StudentProgress
        builder.Entity<StudentProgress>(entity =>
        {
            // entity.HasOne(e => e.Student).WithMany(s => s.StudentProgress).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.ModuleContent).WithMany(m => m.StudentProgress).HasForeignKey(e => e.ModuleContentId);
        });

        // Configure TeacherCourse
        builder.Entity<TeacherCourse>(entity =>
        {
            entity.HasOne(e => e.Teacher).WithMany(t => t.Courses).HasForeignKey(e => e.TeacherId);
            // entity.HasOne(e => e.Course).WithMany(c => c.Teachers).HasForeignKey(e => e.CourseId);
        });

        // Configure StudentCourse
        builder.Entity<StudentCourse>(entity =>
        {
            entity.HasOne(e => e.Student).WithMany(s => s.Courses).HasForeignKey(e => e.StudentId);
            // entity.HasOne(e => e.Course).WithMany(c => c.Students).HasForeignKey(e => e.CourseId);
        });

        // Configure CourseResource
        builder.Entity<CourseResource>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Course).WithMany(c => c.Resources).HasForeignKey(e => e.CourseId);
        });

        // Configure AssignmentSubmission
        builder.Entity<AssignmentSubmission>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Assignment).WithMany(a => a.Submissions).HasForeignKey(e => e.AssignmentId);
            entity.HasOne(e => e.Student).WithMany(s => s.AssignmentSubmissions).HasForeignKey(e => e.StudentId);
            entity.HasOne(e => e.GradedByTeacher).WithMany().HasForeignKey(e => e.GradedByTeacherId);
        });

        // Configure AssignmentRubric
        builder.Entity<AssignmentRubric>(entity =>
        {
            entity.Property(e => e.Criterion).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Assignment).WithMany(a => a.Rubrics).HasForeignKey(e => e.AssignmentId);
        });

        // Configure RubricLevel
        builder.Entity<RubricLevel>(entity =>
        {
            entity.Property(e => e.LevelName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Rubric).WithMany(r => r.Levels).HasForeignKey(e => e.RubricId);
        });

        // Configure SubmissionComment
        builder.Entity<SubmissionComment>(entity =>
        {
            entity.Property(e => e.Comment).IsRequired();
            entity.HasOne(e => e.Submission).WithMany(s => s.Comments).HasForeignKey(e => e.SubmissionId);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
        });

        // Configure ExamResult
        builder.Entity<ExamResult>(entity =>
        {
            entity.Property(e => e.Grade).IsRequired().HasMaxLength(10);
            entity.HasOne(e => e.Exam).WithMany(e => e.Results).HasForeignKey(e => e.ExamId);
            entity.HasOne(e => e.Student).WithMany(s => s.ExamResults).HasForeignKey(e => e.StudentId);
        });

        // Configure ExamQuestion
        builder.Entity<ExamQuestion>(entity =>
        {
            entity.Property(e => e.Question).IsRequired();
            entity.HasOne(e => e.Exam).WithMany(e => e.Questions).HasForeignKey(e => e.ExamId);
        });

        // Configure AnsweredQuestion
        builder.Entity<AnsweredQuestion>(entity =>
        {
            entity.Property(e => e.Answer).IsRequired();
            entity.HasOne(e => e.ExamQuestion).WithMany(q => q.Answers).HasForeignKey(e => e.ExamQuestionId);
            entity.HasOne(e => e.ExamResult).WithMany(r => r.AnsweredQuestions).HasForeignKey(e => e.ExamResultId);
        });

        // Configure AcademicYear
        builder.Entity<AcademicYear>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.School).WithMany(s => s.AcademicYears).HasForeignKey(e => e.SchoolId);
        });

        // Configure Term
        builder.Entity<Term>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.AcademicYear).WithMany(a => a.Terms).HasForeignKey(e => e.AcademicYearId);
        });

        // Configure AttendanceSummary (Restrict: Subject links to Class — avoids SQL Server multiple cascade paths)
        builder.Entity<AttendanceSummary>(entity =>
        {
            entity.HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Subject).WithMany().HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Subject
        builder.Entity<Subject>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.Class).WithMany(c => c.Subjects).HasForeignKey(e => e.ClassId);
            entity.HasOne(e => e.Department).WithMany(d => d.Subjects).HasForeignKey(e => e.DepartmentId);
        });

        // Configure Stream
        builder.Entity<MaphunziroBlackboard.Domain.Entities.Stream>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.Class).WithMany(c => c.Streams).HasForeignKey(e => e.ClassId);
        });

        // SQL Server: multiple cascade paths — keep AspNet Identity cascades only
        foreach (var fk in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            if (fk.DeleteBehavior != DeleteBehavior.Cascade)
                continue;
            var principalTable = fk.PrincipalEntityType.GetTableName();
            if (principalTable != null && principalTable.StartsWith("AspNet", StringComparison.OrdinalIgnoreCase))
                continue;
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
