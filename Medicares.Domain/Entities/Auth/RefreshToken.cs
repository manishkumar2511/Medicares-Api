using Medicares.Domain.Base;
namespace Medicares.Domain.Entities.Auth;

public class RefreshToken : BaseOwnerEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive => DeletedAt == null && ExpiresAt > DateTime.UtcNow;
    
    public ApplicationUser User { get; set; } = null!;
}
