using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.DTO;

namespace Medicares.Application.Contracts.Interfaces.Repositories;

public interface IIdentityService
{
    IQueryable<ApplicationUser> Users { get; }
    IQueryable<Role> Roles { get; }

    Task<Guid> GetRoleIdAsync(Guid userId, string userRole);
    Task<LoginResult> LoginAsync(string email, string password, bool require2FA = true, CancellationToken ct = default);
    Task<(bool Success, string? Error)> SendMfaCodeAsync(string email, CancellationToken ct = default);
    Task<LoginResult> Verify2FACodeAsync(string email, string code, CancellationToken ct = default);
    Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<(ApplicationUser? User, string? Error)> CreateUserAsync(UserDto userRequest, string password, Guid? addressId, CancellationToken ct = default);
    Task<(bool Success, string? Error)> LogoutAsync(Guid userId, CancellationToken ct = default);

    // Super Admin specific methods (Owner)
    Task<(Owner? Owner, string? Error)> CreateOwnerAsync(Owner owner, CancellationToken ct = default);
    Task<(bool Success, string? Error)> DeactivateOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<(bool Success, string? Error)> ActivateOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<IEnumerable<Owner>> GetAllOwnersAsync(CancellationToken ct = default);
    
    Task<(bool success, string? Error)> SendResetPassswordAsync(string email, string? source = null, CancellationToken ct = default);
    Task<(bool Success, string? Error)> VerifyResetTokenAsync(string email, string token, CancellationToken ct = default);
    Task<LoginResult> ResetPasswordAsync(ResetPasswordRequest req, CancellationToken ct = default);
    Task<(bool success, string? Error)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);

    //profile info
    Task<ApplicationUser?> GetProfileAsync(Guid id, CancellationToken ct = default);
    Task UpdateUserProfile(ApplicationUser user, CancellationToken ct = default);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    IQueryable<ApplicationUser> GetAllUserByRole(string role);
}
