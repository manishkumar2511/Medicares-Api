using Medicares.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Medicares.Application.Contracts.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe);
    Task SignOutAsync();
}
