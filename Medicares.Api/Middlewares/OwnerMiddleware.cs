using Medicares.Domain.Entities.Auth;
using Medicares.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Medicares.Api.Middlewares;

public class OwnerMiddleware
{
    private readonly RequestDelegate _next;

    public OwnerMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx, ApplicationDbContext db)
    {
        Guid? resolvedOwnerId = null;

        if (ctx.Request.Headers.TryGetValue("X-Owner-Id", out StringValues headerVal)
            && Guid.TryParse(headerVal.FirstOrDefault(), out Guid headerTid))
        {
            resolvedOwnerId = headerTid;
        }

        if (resolvedOwnerId == null)
        {
            string? claimOwner = ctx.User?.FindFirst("ownerId")?.Value; 
            if (Guid.TryParse(claimOwner, out Guid claimTid))
            {
                resolvedOwnerId = claimTid;
            }
        }

        db.CurrentOwnerId = resolvedOwnerId;
        ctx.Items["OwnerId"] = resolvedOwnerId;

        if (resolvedOwnerId != null)
        {
            Owner? owner = await db.Owners.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == resolvedOwnerId);
            if (owner != null && !owner.IsActive)
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                await ctx.Response.WriteAsync("Owner is inactive.");
                return;
            }
        }
        await _next(ctx);
    }
}
