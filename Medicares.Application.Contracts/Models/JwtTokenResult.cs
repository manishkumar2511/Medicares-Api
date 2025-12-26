namespace Medicares.Application.Contracts.Models;

public class JwtTokenResult
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
