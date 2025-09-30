using CodeNudge.Core.Domain.Entities;
using System.Security.Claims;

namespace CodeNudge.Core.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
}
