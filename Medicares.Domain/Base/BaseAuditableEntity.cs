namespace Medicares.Domain.Base
{
    public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity, IDeleteEntity, IOwnerEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid OwnerId { get; set; }
    }
}
