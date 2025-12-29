namespace Medicares.Application.Features.Auth.Verify2FACode
{
    public readonly record struct Verify2FACodeRequest(string Email, string Code);
    
}
