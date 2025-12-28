using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Medicares.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        bool isValid = await _userManager.CheckPasswordAsync(user, password);
        return isValid;
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if (user == null) return SignInResult.Failed;

        SignInResult result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        return result;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
