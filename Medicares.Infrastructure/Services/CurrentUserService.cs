using Medicares.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Medicares.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            string? userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
        }
    }

    public Guid? OwnerId
    {
        get
        {
            string? ownerIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("ownerId");
            return Guid.TryParse(ownerIdString, out Guid ownerId) ? ownerId : null;
        }
    }
}
