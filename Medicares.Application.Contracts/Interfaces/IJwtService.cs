using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using System.Security.Claims;

namespace Medicares.Application.Contracts.Interfaces;

public interface IJwtService
{
    JwtTokenResult GenerateAccessToken(ApplicationUser user, string email, string role, Guid ownerId, Guid userRoleId);
    RefreshToken GenerateRefreshToken(Guid userId, Guid ownerId);
    ClaimsPrincipal? ValidateToken(string token);
    Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct = default);
    Task RevokeRefreshTokensAsync(Guid userId, CancellationToken ct = default);
}
