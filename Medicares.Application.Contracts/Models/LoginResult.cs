namespace Medicares.Application.Contracts.Models;

public class LoginResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Error { get; set; }
    public bool RequiresTwoFactor { get; set; }
}
