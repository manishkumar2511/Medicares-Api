using Medicares.Domain.Base;
namespace Medicares.Domain.Entities.Auth;

public class RefreshToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public new Guid OwnerId { get; set; } // Changed from TenantId to OwnerId
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive => DeletedAt == null && ExpiresAt > DateTime.UtcNow;
    
    public ApplicationUser User { get; set; } = null!;
}
