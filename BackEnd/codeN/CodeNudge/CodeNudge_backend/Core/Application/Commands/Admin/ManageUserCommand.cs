using MediatR;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.Admin;

public class ActivateUserCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
}

public class DeactivateUserCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class UpdateUserRoleCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
    public string AdminUserId { get; set; } = string.Empty;
}

public class DeleteUserCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
