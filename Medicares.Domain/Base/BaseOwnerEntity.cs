namespace Medicares.Domain.Base
{
    public abstract class BaseOwnerEntity : BaseAuditableEntity, IOwnerEntity
    {
        public Guid OwnerId { get; set; }
    }
}
