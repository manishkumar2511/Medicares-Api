using System.Security.Claims;

namespace Medicares.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }

    public static Guid GetOwnerId(this ClaimsPrincipal principal)
    {
        string? ownerIdClaim = principal.FindFirst("OwnerId")?.Value;
        return Guid.TryParse(ownerIdClaim, out Guid ownerId) ? ownerId : Guid.Empty;
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
    }
}
