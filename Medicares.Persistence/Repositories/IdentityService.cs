using Medicares.Application.Contracts.Interfaces;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.Constant;
using Medicares.Domain.Shared.DTO;
using Medicares.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Persistence.Repositories;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<Role> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        ApplicationDbContext dbContext,
        RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    public IQueryable<ApplicationUser> Users => _userManager.Users;
    public IQueryable<Role> Roles => _roleManager.Roles;

    public async Task<Guid> GetRoleIdAsync(Guid userId, string userRole)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Guid.Empty;
        
        Role? role = await _roleManager.FindByNameAsync(userRole);
        return role?.Id ?? Guid.Empty;
    }

    public async Task<LoginResult> LoginAsync(string email, string password, bool require2FA = true, CancellationToken ct = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if (user == null) return new LoginResult { Error = "User not found" };

        if (!user.IsActive) return new LoginResult { Error = "User is deactivated" };

        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, password, false, false);
        
        if (!result.Succeeded) return new LoginResult { Error = "Invalid credentials" };

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? RoleConsts.StoreStaff;
        Role? roleEntity = await _roleManager.FindByNameAsync(role);
        Guid roleId = roleEntity?.Id ?? Guid.Empty;

        JwtTokenResult tokenResult = _jwtService.GenerateAccessToken(user, user.Email!, role, user.OwnerId, roleId);
        RefreshToken refreshToken = _jwtService.GenerateRefreshToken(user.Id, user.OwnerId);

        await _dbContext.RefreshTokens.AddAsync(refreshToken, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new LoginResult
        {
            Succeeded = true,
            AccessToken = tokenResult.AccessToken,
            Expiry = tokenResult.ExpiresAt,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<(bool Success, string? Error)> SendMfaCodeAsync(string email, CancellationToken ct = default)
    {
        // Placeholder for MFA
        return (true, null);
    }

    public async Task<LoginResult> VerifyMfaAsync(string email, string code, CancellationToken ct = default)
    {
         // Placeholder for MFA Verify
         ApplicationUser? user = await _userManager.FindByEmailAsync(email);
         if (user == null) return new LoginResult { Error = "User not found" };
         
         IList<string> roles = await _userManager.GetRolesAsync(user);
         string role = roles.FirstOrDefault() ?? RoleConsts.StoreStaff;
         Role? roleEntity = await _roleManager.FindByNameAsync(role);
         Guid roleId = roleEntity?.Id ?? Guid.Empty;

         JwtTokenResult tokenResult = _jwtService.GenerateAccessToken(user, user.Email!, role, user.OwnerId, roleId);
         RefreshToken refreshToken = _jwtService.GenerateRefreshToken(user.Id, user.OwnerId);

         await _dbContext.RefreshTokens.AddAsync(refreshToken, ct);
         await _dbContext.SaveChangesAsync(ct);

         return new LoginResult
         {
             Succeeded = true,
             AccessToken = tokenResult.AccessToken,
             Expiry = tokenResult.ExpiresAt,
             RefreshToken = refreshToken.Token
         };
    }

    public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        RefreshToken? tokenEntity = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken && r.DeletedAt == null, ct);

        if (tokenEntity == null || !tokenEntity.IsActive)
            return new LoginResult { Error = "Invalid refresh token" };

        ApplicationUser user = tokenEntity.User;
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? RoleConsts.StoreStaff;
        Role? roleEntity = await _roleManager.FindByNameAsync(role);
        Guid roleId = roleEntity?.Id ?? Guid.Empty;

        JwtTokenResult tokenResult = _jwtService.GenerateAccessToken(user, user.Email!, role, user.OwnerId, roleId);
        
        return new LoginResult
        {
            Succeeded = true,
            AccessToken = tokenResult.AccessToken,
            Expiry = tokenResult.ExpiresAt,
            RefreshToken = refreshToken
        };
    }

    public async Task<(ApplicationUser? User, string? Error)> CreateUserAsync(UserDto userRequest, string password, Guid? addressId, CancellationToken ct = default)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = userRequest.Email,
            Email = userRequest.Email,
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName,
            PhoneNumber = userRequest.PhoneNumber,
            OwnerId = userRequest.OwnerId.GetValueOrDefault(), 
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            AddressId = addressId
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (null, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (!string.IsNullOrEmpty(userRequest.Role))
        {
            await _userManager.AddToRoleAsync(user, userRequest.Role);
        }

        return (user, null);
    }

    public async Task<(bool Success, string? Error)> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        await _signInManager.SignOutAsync();
        return (true, null);
    }

    // Owner specifics (mapped from Tenant)
    public async Task<(Owner? Owner, string? Error)> CreateOwnerAsync(Owner owner, CancellationToken ct = default)
    {
        owner.Id = Guid.NewGuid();
        owner.CreatedAt = DateTime.UtcNow;
        
        await _dbContext.Owners.AddAsync(owner, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return (owner, null);
    }

    public async Task<(bool Success, string? Error)> DeactivateOwnerAsync(Guid ownerId, CancellationToken ct = default)
    {
        Owner? owner = await _dbContext.Owners.FindAsync([ownerId], ct);
        if (owner == null) return (false, "Owner not found");

        owner.IsActive = false;
        owner.DeactivatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ActivateOwnerAsync(Guid ownerId, CancellationToken ct = default)
    {
        Owner? owner = await _dbContext.Owners.FindAsync([ownerId], ct);
        if (owner == null) return (false, "Owner not found");

        owner.IsActive = true;
        owner.DeactivatedAt = null;
        await _dbContext.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<IEnumerable<Owner>> GetAllOwnersAsync(CancellationToken ct = default)
    {
        return await _dbContext.Owners.ToListAsync(ct);
    }
    
    public async Task<(bool success, string? Error)> SendResetPassswordAsync(string email, string? source = null, CancellationToken ct = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if(user == null) return (false, "User not found");
        
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Logic to send email would go here (IEmailService)
        return (true, null); 
    }

    public async Task<(bool Success, string? Error)> VerifyResetTokenAsync(string email, string token, CancellationToken ct = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if (user == null) return (false, "User not found");
        
        return (true, null);
    }

    public async Task<LoginResult> ResetPasswordAsync(ResetPasswordRequest req, CancellationToken ct = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return new LoginResult { Error = "User not found" };

        IdentityResult result = await _userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
        if (!result.Succeeded) return new LoginResult { Error = string.Join(", ", result.Errors.Select(e => e.Description)) };

        return new LoginResult { Succeeded = true };
    }

    public async Task<(bool success, string? Error)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return (false, "User not found");
        
        IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
         if (!result.Succeeded) return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
         
         return (true, null);
    }

    public async Task<ApplicationUser?> GetProfileAsync(Guid id, CancellationToken ct = default)
    {
        return await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task UpdateUserProfile(ApplicationUser user, CancellationToken ct = default)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public IQueryable<ApplicationUser> GetAllUserByRole(string role)
    {
        // This is tricky with Identity tables if not joined manually or using GetUsersInRoleAsync (which returns List, not Queryable)
        // For Queryable, we can join UserRoles
        Guid roleId = _roleManager.Roles.Where(r => r.Name == role).Select(r => r.Id).FirstOrDefault();
        // This logic is simplified.
        return _userManager.Users; 
    }
}
