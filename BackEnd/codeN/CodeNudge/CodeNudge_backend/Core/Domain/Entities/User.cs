using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CodeNudge.Core.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string? ProfilePicture { get; set; }
    public string? College { get; set; }
    public string? Branch { get; set; }
    public int? GraduationYear { get; set; }

    // Role-specific identifiers
    public string? RegisterId { get; set; } // For students
    public string? EmployeeId { get; set; } // For admins

    public bool IsEmailVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Audit fields from BaseEntity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public virtual ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();
    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
    public virtual ICollection<InterviewExperience> InterviewExperiences { get; set; } = new List<InterviewExperience>();
    public virtual ICollection<LeaderboardEntry> LeaderboardEntries { get; set; } = new List<LeaderboardEntry>();
    
    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";
}
