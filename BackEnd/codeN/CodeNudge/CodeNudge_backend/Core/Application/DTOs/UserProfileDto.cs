using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? ProfilePicture { get; set; }
    public string? College { get; set; }
    public string? Branch { get; set; }
    public int? GraduationYear { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserStatisticsDto Statistics { get; set; } = new();
}

public class UserStatisticsDto
{
    public int TotalSubmissions { get; set; }
    public int AcceptedSubmissions { get; set; }
    public int TotalQuestionsSolved { get; set; }
    public int EasyQuestionsSolved { get; set; }
    public int MediumQuestionsSolved { get; set; }
    public int HardQuestionsSolved { get; set; }
    public double AccuracyRate { get; set; }
    public int TotalInterviewsHosted { get; set; }
    public int TotalInterviewsParticipated { get; set; }
    public int TotalExperiencesShared { get; set; }
    public int TotalExperienceLikes { get; set; }
    public int CurrentRank { get; set; }
    public int TotalPoints { get; set; }
    public int WeeklyChallengesParticipated { get; set; }
    public int WeeklyChallengesWon { get; set; }
}

public class UserActivityDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserProgressDto
{
    public List<CategoryProgressDto> CategoryProgress { get; set; } = new();
    public List<DifficultyProgressDto> DifficultyProgress { get; set; } = new();
    public List<MonthlyProgressDto> MonthlyProgress { get; set; } = new();
    public List<StreakDto> SolvingStreak { get; set; } = new();
}



public class DifficultyProgressDto
{
    public string Difficulty { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int SolvedQuestions { get; set; }
    public double ProgressPercentage { get; set; }
}

public class MonthlyProgressDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int QuestionsSolved { get; set; }
    public int SubmissionsMade { get; set; }
    public int InterviewsParticipated { get; set; }
}

public class StreakDto
{
    public DateTime Date { get; set; }
    public int QuestionsSolved { get; set; }
    public bool HasActivity { get; set; }
}
