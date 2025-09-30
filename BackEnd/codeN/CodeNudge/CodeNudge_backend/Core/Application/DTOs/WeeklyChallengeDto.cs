namespace CodeNudge.Core.Application.DTOs;

public class WeeklyChallengeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public int PrizePool { get; set; }
    public bool IsUserParticipating { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ChallengeQuestionDto> Questions { get; set; } = new();
    public ChallengeStatus Status { get; set; }
}

public class WeeklyChallengeListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int CurrentParticipants { get; set; }
    public int MaxParticipants { get; set; }
    public int PrizePool { get; set; }
    public bool IsUserParticipating { get; set; }
    public ChallengeStatus Status { get; set; }
    public int QuestionsCount { get; set; }
}

public class ChallengeQuestionDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Points { get; set; }
    public int OrderIndex { get; set; }
    public bool IsUserSolved { get; set; }
    public int? UserBestScore { get; set; }
}

public class ChallengeParticipantDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public int TotalScore { get; set; }
    public int QuestionsSolved { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastSubmissionAt { get; set; }
    public int Rank { get; set; }
}

public class ChallengeLeaderboardDto
{
    public Guid ChallengeId { get; set; }
    public string ChallengeTitle { get; set; } = string.Empty;
    public List<ChallengeParticipantDto> Participants { get; set; } = new();
    public int TotalParticipants { get; set; }
    public ChallengeParticipantDto? CurrentUserRank { get; set; }
}

public enum ChallengeStatus
{
    Upcoming,
    Active,
    Completed,
    Cancelled
}
