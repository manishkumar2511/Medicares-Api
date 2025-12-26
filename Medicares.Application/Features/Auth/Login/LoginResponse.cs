using Medicares.Domain.Shared.DTO;

namespace Medicares.Application.Features.Auth.Login;

public class LoginResponse
{
    public bool RequiresMfa { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public UserDto? User { get; set; }
}
