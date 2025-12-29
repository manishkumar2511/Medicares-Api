using Medicares.Domain.Entities.Auth;

namespace Medicares.Application.Contracts.Models;

public class LoginResult
{
    public ApplicationUser? User { get; set; }
    public string? Role { get; set; }
    public JwtTokenResult? Token { get; set; }
    public RefreshToken? RefreshToken { get; set; }
    public string? Error { get; set; }
    public bool RequiresMfa { get; set; }
}
