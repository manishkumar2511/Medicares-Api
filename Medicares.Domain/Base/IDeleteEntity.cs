namespace Medicares.Domain.Base
{
    public interface IDeleteEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }
}
