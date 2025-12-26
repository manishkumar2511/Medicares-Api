using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Medicares.Application.Contracts.Interfaces;
using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using Medicares.Infrastructure.Settings;
using Medicares.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Medicares.Infrastructure.Services;

public class JwtService(ApplicationDbContext db, IOptions<JwtSettings> jwtSettings) : IJwtService
{
    public JwtTokenResult GenerateAccessToken(ApplicationUser user, string email, string role, Guid ownerId, Guid userRoleId)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Value.Secret ??
            "d9f4K!2mQ8v@5xT1zR7y#P0wC3sqscgy");

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new("ownerId", ownerId.ToString()), 
            new("roleId", userRoleId.ToString()),
            new("jti", Guid.NewGuid().ToString()), 
            new("userProfileImage", user.ProfilePictureUrl ?? String.Empty)
        ];

        DateTime expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpiryMinutes); // Short-lived access token

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return new JwtTokenResult
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        };
    }

    public RefreshToken GenerateRefreshToken(Guid userId, Guid ownerId)
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OwnerId = ownerId, // Changed from TenantId to OwnerId
            Token = Convert.ToBase64String(randomNumber),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(14)
        };
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Value.Secret ??
            "d9f4K!2mQ8v@5xT1zR7y#P0wC3sqscgy");

        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Value.Issuer,
                ValidAudience = jwtSettings.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        return await db.RefreshTokens
            .AnyAsync(r => r.UserId == userId && r.Token == refreshToken && r.IsActive, ct);
    }

    public async Task RevokeRefreshTokensAsync(Guid userId, CancellationToken ct = default)
    {
        List<RefreshToken> tokens = await db.RefreshTokens
            .Where(r => r.UserId == userId && r.DeletedAt == null)
            .ToListAsync(ct);

        foreach (RefreshToken token in tokens)
        {
            token.DeletedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
    }
}
