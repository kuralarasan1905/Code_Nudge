using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Core.Domain.Enums;
using System.Security.Claims;

namespace CodeNudge.Presentation.Hubs;

[Authorize]
public class InterviewHub : Hub
{
    private readonly CodeNudgeDbContext _context;
    private readonly ILogger<InterviewHub> _logger;

    public InterviewHub(CodeNudgeDbContext context, ILogger<InterviewHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task JoinInterviewRoom(string roomCode)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var interview = await _context.InterviewSessions
                .Include(i => i.HostUser)
                .Include(i => i.ParticipantUser)
                .FirstOrDefaultAsync(i => i.RoomCode == roomCode);

            if (interview == null)
            {
                await Clients.Caller.SendAsync("Error", "Interview room not found");
                return;
            }

            var userGuid = Guid.Parse(userId);
            var isHost = interview.HostUserId == userGuid;
            var isParticipant = interview.ParticipantUserId == userGuid;

            if (!isHost && !isParticipant)
            {
                // Check if user can join as participant
                if (interview.ParticipantUserId == null && interview.IsPublic)
                {
                    interview.ParticipantUserId = userGuid;
                    await _context.SaveChangesAsync();
                    isParticipant = true;
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "You are not authorized to join this interview");
                    return;
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            var userInfo = new
            {
                UserId = userId,
                Name = Context.User?.FindFirst(ClaimTypes.Name)?.Value,
                Role = isHost ? "Host" : "Participant",
                IsHost = isHost
            };

            await Clients.Group(roomCode).SendAsync("UserJoined", userInfo);
            await Clients.Caller.SendAsync("JoinedRoom", new
            {
                RoomCode = roomCode,
                Interview = new
                {
                    interview.Id,
                    interview.Title,
                    interview.Description,
                    interview.ScheduledAt,
                    interview.Duration,
                    interview.Status,
                    Host = interview.HostUser.FullName,
                    Participant = interview.ParticipantUser?.FullName
                },
                UserRole = isHost ? "Host" : "Participant"
            });

            _logger.LogInformation("User {UserId} joined interview room {RoomCode}", userId, roomCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining interview room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to join interview room");
        }
    }

    public async Task LeaveInterviewRoom(string roomCode)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

            var userInfo = new
            {
                UserId = userId,
                Name = Context.User?.FindFirst(ClaimTypes.Name)?.Value
            };

            await Clients.Group(roomCode).SendAsync("UserLeft", userInfo);

            _logger.LogInformation("User {UserId} left interview room {RoomCode}", userId, roomCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving interview room {RoomCode}", roomCode);
        }
    }

    public async Task StartInterview(string roomCode)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var interview = await _context.InterviewSessions
                .FirstOrDefaultAsync(i => i.RoomCode == roomCode);

            if (interview == null)
            {
                await Clients.Caller.SendAsync("Error", "Interview not found");
                return;
            }

            var userGuid = Guid.Parse(userId);
            if (interview.HostUserId != userGuid)
            {
                await Clients.Caller.SendAsync("Error", "Only the host can start the interview");
                return;
            }

            interview.Status = InterviewStatus.InProgress;
            interview.StartedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await Clients.Group(roomCode).SendAsync("InterviewStarted", new
            {
                StartedAt = interview.StartedAt,
                Duration = interview.Duration
            });

            _logger.LogInformation("Interview {InterviewId} started by user {UserId}", interview.Id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting interview in room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to start interview");
        }
    }

    public async Task EndInterview(string roomCode)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var interview = await _context.InterviewSessions
                .FirstOrDefaultAsync(i => i.RoomCode == roomCode);

            if (interview == null)
            {
                await Clients.Caller.SendAsync("Error", "Interview not found");
                return;
            }

            var userGuid = Guid.Parse(userId);
            if (interview.HostUserId != userGuid)
            {
                await Clients.Caller.SendAsync("Error", "Only the host can end the interview");
                return;
            }

            interview.Status = InterviewStatus.Completed;
            interview.EndedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await Clients.Group(roomCode).SendAsync("InterviewEnded", new
            {
                EndedAt = interview.EndedAt
            });

            _logger.LogInformation("Interview {InterviewId} ended by user {UserId}", interview.Id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending interview in room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to end interview");
        }
    }

    public async Task SendMessage(string roomCode, string message)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var messageData = new
            {
                UserId = userId,
                UserName = userName,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(roomCode).SendAsync("MessageReceived", messageData);

            _logger.LogInformation("Message sent in room {RoomCode} by user {UserId}", roomCode, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task ShareCode(string roomCode, string code, string language)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var codeData = new
            {
                UserId = userId,
                UserName = userName,
                Code = code,
                Language = language,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(roomCode).SendAsync("CodeShared", codeData);

            _logger.LogInformation("Code shared in room {RoomCode} by user {UserId}", roomCode, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing code in room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to share code");
        }
    }

    public async Task UpdateInterviewNotes(string roomCode, string notes)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            var interview = await _context.InterviewSessions
                .FirstOrDefaultAsync(i => i.RoomCode == roomCode);

            if (interview == null)
            {
                await Clients.Caller.SendAsync("Error", "Interview not found");
                return;
            }

            var userGuid = Guid.Parse(userId);
            if (interview.HostUserId != userGuid)
            {
                await Clients.Caller.SendAsync("Error", "Only the host can update notes");
                return;
            }

            interview.Notes = notes;
            await _context.SaveChangesAsync();

            await Clients.Group(roomCode).SendAsync("NotesUpdated", new
            {
                Notes = notes,
                UpdatedBy = Context.User?.FindFirst(ClaimTypes.Name)?.Value,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Interview notes updated in room {RoomCode} by user {UserId}", roomCode, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notes in room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("Error", "Failed to update notes");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} disconnected from interview hub", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
